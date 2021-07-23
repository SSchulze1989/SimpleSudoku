using SimpleSudoku.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace SimpleSudoku.ViewModels
{
    public class SudokuFieldViewModel : ViewModelBase
    {
        private SudokuSolver Solver { get; set; }
        private SudokuField FieldModel { get; set; }
        public SudokuCellViewModel[,] Cells { get; }
        public SudokuCellViewModel[] Cells1D => Cells.To1DArray();

        private int _level;
        public int Level { get => _level; set => SetValue(ref _level, value); }

        public ICommand New { get; }
        public ICommand SolveNext { get; }
        public ICommand Reset { get; }
        public ICommand Save { get; }

        public SudokuFieldViewModel()
        {
            New = new RelayCommand(o => NewAction(), o => true);
            SolveNext = new RelayCommand(o => SolveNextAction(), o => FieldModel != null);
            Reset = new RelayCommand(o => ResetAction(), o => FieldModel != null);
            Save = new RelayCommand(o => SaveAction(), o => FieldModel != null);

            Cells = new SudokuCellViewModel[9, 9];
            for (int i = 0; i < Cells.GetLength(0); i++)
            {
                for (int j = 0; j < Cells.GetLength(1); j++)
                {
                    Cells[i, j] = new SudokuCellViewModel();
                }
            }

            SetFieldModel(new SudokuField());
        }

        public SudokuFieldViewModel(SudokuField field) : this()
        {
            SetFieldModel(field);
        }

        private void SetFieldModel(SudokuField field)
        {
            FieldModel = field;
            SetCells(field);
            Solver = new SudokuSolver(field);
            OnPropertyChanged(null);
        }

        private static SudokuField GetDefaultField()
        {
            var creator = new SudokuCreator();
            return creator.Create();

            var trueValues = new int[9, 9] {
                { 2, 4, 9, 7, 5, 3, 8, 6, 1 },
                { 5, 6, 7, 1, 8, 4, 9, 2, 3 },
                { 3, 1, 8, 6, 9, 2, 4, 7, 5 },
                { 4, 8, 2, 5, 3, 1, 7, 9, 6 },
                { 6, 7, 1, 9, 4, 8, 5, 3, 2 },
                { 9, 3, 5, 2, 7, 6, 1, 4, 8 },
                { 7, 9, 6, 8, 2, 5, 3, 1, 4 },
                { 1, 5, 3, 4, 6, 7, 2, 8, 9 },
                { 8, 2, 4, 3, 1, 9, 6, 5, 7 }
            };

            var cells = new SudokuCell[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    cells[i, j] = new SudokuCell(i, j, trueValues[i, j] == 0 ? (int?)null : trueValues[i, j]);
                    cells[i, j].SetTrueValue(trueValues[i, j]);
                    cells[i, j].Notes.SetAll();
                }
            }

            var field = new SudokuField(cells);

            var validator = new SudokuValidator(field);
            if (validator.ValidateAll() == false)
            {
                var exception = new Exception("Validation failed!");
                foreach (var (fail, index) in validator.ValidationFails.WithIndex())
                {
                    exception.Data.Add(index, fail);
                }
                throw exception;
            }

            var cellValues = new int[9, 9] {
                { 0, 0, 0, 0, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 1, 0, 0, 0 }
            };

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (cellValues[i,j] == 0)
                    {
                        field.Cells[i, j].Value = null;
                    }
                }
            }

            return field;
        }

        private void SetCells(SudokuField field)
        {
            foreach(var cell in field.Cells.To1DArray())
            {
                Cells[cell.Row, cell.Column].SetCell(cell);
            }
            OnPropertyChanged(nameof(Cells));
        }

        private void NewAction()
        {
            var creator = new SudokuCreator();
            SetFieldModel(creator.Create());
        }

        private void SolveNextAction()
        {
            int level;
            Solver.Next(out level);
            Level = level;
            foreach(var cell in Cells1D)
            {
                cell.Notes.SetValues(Solver.SolverField.Cells[cell.Row, cell.Column].Notes.Values);
                //if (cell.Notes.Values.Count == 1)
                //{
                //    cell.Value = cell.Notes.Values.First();
                //}
            }
        }

        private void ResetAction()
        {
            Solver.Reset();
            foreach (var cell in Cells1D)
            {
                if (cell.StartCell == false)
                {
                    cell.Value = null;
                }
                cell.Notes.Clear();
                //if (cell.Notes.Values.Count == 1)
                //{
                //    cell.Value = cell.Notes.Values.First();
                //}
            }
        }

        public string SaveAction()
        {
            var data = FieldModel.Save();
            return data;
        }

        public void LoadAction(string data)
        {
            var field = SudokuField.Load(data);
            SetFieldModel(field);
        }
    }
}
