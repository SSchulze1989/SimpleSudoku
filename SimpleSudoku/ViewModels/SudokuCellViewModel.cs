using SimpleSudoku.Game;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SimpleSudoku.ViewModels
{
    public class SudokuCellViewModel : ViewModelBase
    {
        private SudokuCell CellModel { get; set; }
        public int? Value { get => CellModel.Value; set { CellModel.Value = value; OnPropertyChanged(); } }
        public int Row => CellModel.Row;
        public int Column => CellModel.Column;
        public CellNoteViewModel Notes { get; }

        private ObservableCollection<ValidationFail> _validationFails;
        public ObservableCollection<ValidationFail> ValidationFails { get => _validationFails; set => SetValue(ref _validationFails, value); }

        public SudokuCellViewModel()
        {
            Notes = new CellNoteViewModel();
            ValidationFails = new ObservableCollection<ValidationFail>();
        }

        public void SetCell(SudokuCell cell)
        {
            CellModel = cell;
            Notes.SetNoteModel(CellModel.Notes);
            OnPropertyChanged();
        }
    }
}
