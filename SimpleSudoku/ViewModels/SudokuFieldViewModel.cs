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

        public SudokuCellViewModel[] Cells1D => Cells.To1DArray();

        public SudokuFieldViewModel() : this(GetDefaultField())
        {
            //Cells = new SudokuCellViewModel[9, 9];
            //for (int i = 0; i < Cells.GetLength(0); i++)
            //{
            //    for (int j = 0; j < Cells.GetLength(1); j++)
            //    {
            //        Cells[i, j] = new SudokuCellViewModel();
            //    }
            //}

            //SudokuCell[,] cells = new SudokuCell[9, 9];
            //for (int i = 0; i < cells.GetLength(0); i++)
            //{
            //    for (int j = 0; j < cells.GetLength(1); j++)
            //    {
            //        cells[i, j] = new SudokuCell(i, j, i * 9 + j);
            //    }
            //}

            //FieldModel = new SudokuField(cells);
            //SetCells(FieldModel);
        }

        private static SudokuField GetDefaultField()
        {
            var cellValues = new int[9, 9] {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 1, 2, 3, 0, 0, 0},
                { 0, 5, 0, 0, 0, 0, 0, 0, 0},
                { 0, 6, 0, 0, 0, 0, 0, 0, 0},
                { 0, 7, 0, 0, 0, 0, 0, 0, 0},
                { 0, 8, 0, 0, 0, 0, 0, 0, 0},
                { 0, 9, 0, 0, 0, 0, 0, 0, 0}
            };

            var cells = new SudokuCell[9, 9];
            for (int i = 0; i<9; i++)
            {
                for (int j = 0; j<9; j++)
                {
                    cells[i, j] = new SudokuCell(i, j, cellValues[i, j] == 0 ? (int?)null : cellValues[i,j]);
                }
            }

            var field = new SudokuField(cells);

            return field;
        }

        public SudokuFieldViewModel(SudokuField field)
        {
            Cells = new SudokuCellViewModel[9, 9];
            for (int i = 0; i < Cells.GetLength(0); i++)
            {
                for (int j = 0; j < Cells.GetLength(1); j++)
                {
                    Cells[i, j] = new SudokuCellViewModel();
                }
            }

            FieldModel = field;
            SetCells(FieldModel);
            var solver = new SudokuSolver(field);
            solver.Solve();
        }

        private void SetCells(SudokuField field)
        {
            foreach(var cell in field.Cells.To1DArray())
            {
                Cells[cell.Row, cell.Column].SetCell(cell);
            }
            OnPropertyChanged(nameof(Cells));
        }
    }
}
