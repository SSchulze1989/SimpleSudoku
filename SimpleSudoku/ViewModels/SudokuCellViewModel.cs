using SimpleSudoku.Game;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SimpleSudoku.ViewModels
{
    public class SudokuCellViewModel : ViewModelBase
    {
        private SudokuCell CellModel { get; set; }
        public int? Value { get => CellModel.Value; set { CellModel.Value = value; OnPropertyChanged(); } }
        public int TrueValue => CellModel.TrueValue;
        public int Row => CellModel.Row;
        public int Column => CellModel.Column;
        public CellNoteViewModel Notes { get; }

        private bool _isReadOnly;
        public bool IsReadOnly { get => _isReadOnly; set => SetValue(ref _isReadOnly, value); }

        public bool Solved => Value == TrueValue || (Notes.Values.Count == 1 && Notes.Values.First() == TrueValue);
        public bool Wrong =>
            (Value != null && Value != TrueValue) ||
            (Notes.Values.Count == 1 && Notes.Values.First() != TrueValue);

        private ObservableCollection<ValidationFail> _validationFails;
        public ObservableCollection<ValidationFail> ValidationFails { get => _validationFails; set => SetValue(ref _validationFails, value); }

        public SudokuCellViewModel()
        {
            Notes = new CellNoteViewModel();
            Notes.PropertyChanged += Notes_PropertyChanged;
            ValidationFails = new ObservableCollection<ValidationFail>();
        }

        private void Notes_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Solved));
            OnPropertyChanged(nameof(Wrong));
        }

        public void SetCell(SudokuCell cell)
        {
            CellModel = cell;
            Notes.SetNoteModel(CellModel.Notes);
            OnPropertyChanged(nameof(Value));
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            switch(propertyName)
            {
                case nameof(Value):
                    OnPropertyChanged(nameof(Solved));
                    OnPropertyChanged(nameof(Wrong));
                    break;
                default:
                    break;
            }
            base.OnPropertyChanged(propertyName);
        }
    }
}
