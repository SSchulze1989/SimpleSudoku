using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SimpleSudoku.Game
{
    public class SudokuCreator
    {
        public int MaxTries { get; set; } = 99999;

        public bool MaxTriesReached { get; private set; }

        public SudokuField Create()
        {
            var rand = new Random();
            var field = new SudokuField();
            var validator = new SudokuValidator(field);
            var tries = 0;

            MaxTriesReached = false;

            // Insert random numbers and check if valid
            while (field.Initialized == false && tries < MaxTries)
            {
                field.ResetCells();
                bool invalid = false;

                for (int row = 0; row < 9; row++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        var tryValues = new List<int>(Enumerable.Range(1, 9));
                        var cell = field.Cells[row, col];
                        tryValues.RemoveMany(field
                            .GetRow(cell.Row)
                            .Where(x => x.Value != null)
                            .Select(x => x.Value.Value));
                        tryValues.RemoveMany(field
                            .GetColumn(cell.Column)
                            .Where(x => x.Value != null)
                            .Select(x => x.Value.Value));
                        tryValues.RemoveMany(field
                            .GetBlock(cell.Row / 3, cell.Column / 3)
                            .To1DArray()
                            .Where(x => x.Value != null)
                            .Select(x => x.Value.Value));

                        while (cell.Value == null || validator.ValidateCell(cell) == false)
                        {
                            int index = rand.Next(tryValues.Count);

                            if (tryValues.Count == 0)
                            {
                                // Invalid field
                                invalid = true;
#if DEBUG
                                //Trace.WriteLine("Invalid state reached :");
                                //DebugPrintField(field);
#endif
                                break;
                            }

                            field.Cells[row, col].Value = tryValues[index];
                            tryValues.RemoveAt(index);
                            if (tries++ >= MaxTries)
                            {
                                MaxTriesReached = true;
                                break;
                            }
                        }
                        if (invalid)
                        {
                            break;
                        }
                    }
                    if (invalid)
                    {
                        break;
                    }
                }

                if (invalid == false)
                {
                    field.SetInitialized();
                }
            }

#if DEBUG
            Trace.WriteLine($"Creator finished after {tries} tries: ");
            DebugPrintField(field);
#endif
            // Set true values
            foreach(var cell in field.Cells.To1DArray())
            {
                cell.SetTrueValue(cell.Value.Value);
            }


            // Randomly remove cell values
            for (int i = 0; i < 1000; i++)
            {
                var row = rand.Next(9);
                var col = rand.Next(9);
                var value = field.Cells[row, col].Value;
                field.Cells[row, col].Value = null;
                var solver = new SudokuSolver(field);
                if (solver.Solve() == false)
                {
                    field.Cells[row, col].Value = value;
                }
            }

            return field;
        }


        private void DebugPrintField(SudokuField field)
        {
            foreach (var printRow in field.Rows)
            {
                Trace.WriteLine("[ " + printRow.Select(x => x.Value?.ToString("0") ?? " ").Aggregate((x, y) => $"{x} {y}") + " ]");
            }
        }
    }
}
