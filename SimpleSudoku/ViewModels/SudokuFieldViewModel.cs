using SimpleSudoku.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSudoku.ViewModels
{
    public class SudokuFieldViewModel : ViewModelBase
    {
        private SudokuField FieldModel { get; }

        public SudokuCellViewModel[,] Cells { get; }

        public SudokuFieldViewModel(SudokuField field)
        {
            Cells = new SudokuCellViewModel[9,9];

            FieldModel = field;
            SetCells(FieldModel);
        }

        private void SetCells(SudokuField field)
        {
            foreach(var cell in field.Cells.To1DArray())
            {
                Cells[cell.Row, cell.Column].SetCell(cell);
            }
        }
    }
}
