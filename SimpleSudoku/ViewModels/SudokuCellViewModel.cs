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
        public int? Value { get => CellModel.Value; set { CellModel.Value = value; OnPropertyChanged(); if (value != null) Hint = null; } }
        public int TrueValue => CellModel.TrueValue;
        public int Row => CellModel.Row;
        public int Column => CellModel.Column;
        public CellNoteViewModel Notes { get; }
        public bool StartCell => CellModel.IsStartCell;
        public bool IsReadOnly => StartCell;

        public bool Solved => Value == TrueValue || (Notes.Values.Count == 1 && Notes.Values.First() == TrueValue);
        public bool Wrong => (Value != null && TrueValue != 0 && Value != TrueValue);

        private int? _hint;
        public int? Hint { get => _hint; set => SetValue(ref _hint, value); }

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
            OnPropertyChanged(nameof(StartCell));
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            switch(propertyName)
            {
                case nameof(Value):
                    OnPropertyChanged(nameof(Solved));
                    OnPropertyChanged(nameof(Wrong));
                    break;
                case nameof(StartCell):
                    OnPropertyChanged(nameof(IsReadOnly));
                    break;
                default:
                    break;
            }
            base.OnPropertyChanged(propertyName);
        }
    }
}
