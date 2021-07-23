using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleSudoku.Game
{
    public class SudokuSolver
    {
        public SudokuField SolverField { get; set; }
        //private static int solverCycle { get; set; }

        public SudokuSolver(SudokuField field)
        {
            SolverField = new SudokuField(field);
            Reset();
        }

        private void SetNotesForCellValue(SudokuCell cell, int value)
        {
            var row = SolverField.GetRow(cell.Row);
            foreach (var rowCell in row.Except(new SudokuCell[] { cell }))
            {
                rowCell.Notes.Remove(value);
            }
            var column = SolverField.GetColumn(cell.Column);
            foreach (var columnCell in column.Except(new SudokuCell[] { cell }))
            {
                columnCell.Notes.Remove(value);
            }
            var block = SolverField.GetBlock(cell.Row / 3, cell.Column / 3);
            foreach (var blockCell in block.To1DArray().Except(new SudokuCell[] { cell }))
            {
                blockCell.Notes.Remove(value);
            }
        }

        public bool Solve()
        {
            while(Next())
            {
                //FillSolutions();
            }

            return CheckSolved();
        }

        public bool Next()
        {
            return Next(out int level);
        }

        public void FillSolutions()
        {
            foreach(var cell in SolverField.Cells.To1DArray().Where(x => x.Notes.Values.Count() == 1))
            {
                cell.Value = cell.Notes.Values.First();
            }
        }

        public bool CheckSolved()
        {
            bool solved = SolverField.Cells.To1DArray().Count(x => x.Value == null && x.Notes.Values.Count() != 1) == 0;
            // validate solution
            var validator = new SudokuValidator(SolverField);
            solved &= validator.ValidateAll();

            return solved;
        }

        public bool Next(out int level)
        {
            var changed = false;

            changed |= GetPossibleValues1();
            level = 1;
            if (changed == false)
            {
                changed |= GetPossibleValues2();
                level = 2;
            }
            if (changed == false)
            { 
                changed |= GetPossibleValues3();
                level = 3;
            }
            if (changed == false)
            {
                level = -1;
            }

            return changed;
        }

        public void Reset()
        {
            foreach (var cell in SolverField.Cells.To1DArray())
            {
                cell.Notes.SetAll();
            }
        }

        public void ApplyNotes(SudokuField field)
        {
            foreach(var trgCell in field.Cells.To1DArray())
            {
                var srcCell = SolverField.Cells[trgCell.Row, trgCell.Column];
                trgCell.Notes.Clear();
                foreach(var value in srcCell.Notes.Values)
                {
                    trgCell.Notes.Add(value);
                }
            }
        }


        /// <summary>
        /// Exclude possible values on first sight using the existing Values
        /// </summary>
        /// <returns><see langword="true"/> if a note hast been changed</returns>
        private bool GetPossibleValues1()
        {
            bool changed = false;
            // iterate over each cell that has a value
            foreach (var cell in SolverField.Cells.To1DArray().Where(x => x.Value != null))
            {
                if (cell.Notes.Values.Count() > 0)
                {
                    cell.Notes.Clear();
                    SetNotesForCellValue(cell, cell.Value.Value);
                    changed = true;
                }
            }
            return changed;
        }

        /// <summary>
        /// Exclude possible by finding unique missing numbers in row/column/block
        /// </summary>
        /// /// <returns><see langword="true"/> if a note hast been changed</returns>
        private bool GetPossibleValues2()
        {
            var changed = false;

            // iterate over each row and find unique values
            foreach(var row in SolverField.Rows)
            {
                // check occurence of each number in either value or notes
                var occurences = Enumerable.Range(1, 9)
                    .Select(x => (value: x, count: row.Count(y => y.Value == x || y.Notes.Values.Contains(x))));
                var unique = occurences.Where(x => x.count == 1);
                foreach(var uniqueValue in unique.Select(x => x.value))
                {
                    // get cell with value in notes and exclude all other possiblities
                    var uniqueCell = row.SingleOrDefault(x => x.Notes.Values.Contains(uniqueValue));
                    if (uniqueCell != null && uniqueCell.Notes.Values.Count() > 1)
                    {
                        uniqueCell.Notes.Clear();
                        uniqueCell.Notes.Add(uniqueValue);
                        changed = true;
                    }
                }
            }

            // iterate over each column and find unique values
            foreach (var column in SolverField.Columns)
            {
                // check occurence of each number in either value or notes
                var occurences = Enumerable.Range(1, 9)
                    .Select(x => (value: x, count: column.Count(y => y.Value == x || y.Notes.Values.Contains(x))));
                var unique = occurences.Where(x => x.count == 1);
                foreach (var uniqueValue in unique.Select(x => x.value))
                {
                    // get cell with value in notes and exclude all other possiblities
                    var uniqueCell = column.SingleOrDefault(x => x.Notes.Values.Contains(uniqueValue));
                    if (uniqueCell != null && uniqueCell.Notes.Values.Count() > 1)
                    {
                        uniqueCell.Notes.Clear();
                        uniqueCell.Notes.Add(uniqueValue);
                        changed = true;
                    }
                }
            }

            // iterate over each block and find unique values
            foreach (var block in SolverField.Blocks)
            {
                // check occurence of each number in either value or notes
                var occurences = Enumerable.Range(1, 9)
                    .Select(x => (value: x, count: block
                        .To1DArray()
                        .Count(y => y.Value == x || y.Notes.Values.Contains(x))
                        ));
                var unique = occurences.Where(x => x.count == 1);
                foreach (var uniqueValue in unique.Select(x => x.value))
                {
                    // get cell with value in notes and exclude all other possiblities
                    var uniqueCell = block
                        .To1DArray()
                        .SingleOrDefault(x => x.Notes.Values.Contains(uniqueValue));
                    if (uniqueCell != null && uniqueCell.Notes.Values.Count() > 1)
                    {
                        uniqueCell.Notes.Clear();
                        uniqueCell.Notes.Add(uniqueValue);
                        changed = true;
                    }
                }
            }

            return changed;
        }

        /// <summary>
        /// Exclude possible values by looking at the sub rows/columns of a block and the respective rows/columns
        /// </summary>
        /// /// <returns><see langword="true"/> if a note hast been changed</returns>
        private bool GetPossibleValues3()
        {
            var changed = false;
            // iterate over each block and then over each sub row/column
            for (int vIndex = 0; vIndex < 3; vIndex++)
            {
                for (int hIndex = 0; hIndex < 3; hIndex++)
                {
                    var block = SolverField.GetBlock(vIndex, hIndex);
                    int vOffset = vIndex * 3;
                    int hOffset = hIndex * 3;

                    // iterate over rows
                    foreach(var (subRow, index) in block.GetRows().WithIndex())
                    {
                        var rowSubRows = Enumerable.Range(0, 3)
                            .Where(x => x != hIndex)
                            .Select(x => SolverField.GetSubRow(index + vOffset, x));
                        // only check cells without value
                        var rowCheckCells = rowSubRows.SelectMany(x => x).Where(x => x.Value == null);

                        var blockSubRows = Enumerable.Range(0, 3)
                            .Where(x => x != index)
                            .Select(x => block.GetRow(x));
                        // only exclude cells without value
                        var blockCheckCells = blockSubRows.SelectMany(x => x).Where(x => x.Value == null);

                        // Check for values that appear in selected subrow but not in checkSubrows
                        var values = subRow.SelectMany(x => x.Notes.Values).Distinct();
                        foreach(var value in values)
                        {
                            // if value is not found in any subrow in row
                            if (rowCheckCells.Any(x => x.Notes.Values.Contains(value)) == false)
                            {
                                if (blockCheckCells.Any(x => x.Notes.Values.Contains(value)))
                                {
                                    blockCheckCells.ForEach(x => x.Notes.Remove(value));
                                    changed = true;
                                }
                            }
                            // if value is not found in any subrow in block
                            if (blockCheckCells.Any(x => x.Notes.Values.Contains(value)) == false)
                            {
                                if (rowCheckCells.Any(x => x.Notes.Values.Contains(value)))
                                {
                                    rowCheckCells.ForEach(x => x.Notes.Remove(value));
                                    changed = true;
                                }
                            }
                        }
                    }

                    foreach (var (subColumn, index) in block.GetColumns().WithIndex())
                    {
                        // get subcolumns to compare with => 2 remaining subcolumns of this column
                        var columnSubColumns = Enumerable.Range(0, 3)
                            .Where(x => x != vIndex)
                            .Select(x => SolverField.GetSubColumn(index + hOffset, x));
                        // only check cells without value
                        var columnCheckCells = columnSubColumns.SelectMany(x => x).Where(x => x.Value == null);

                        // get subcolumns that need to be set => 2 remaining subcolumns in this block
                        var blockSubColumns = Enumerable.Range(0, 3)
                            .Where(x => x != index)
                            .Select(x => block.GetColumn(x));
                        // only exclude cells without value
                        var blockCheckCells = blockSubColumns.SelectMany(x => x).Where(x => x.Value == null);

                        // Check for values that appear in selected subcolumn but not in checkSubcolumns
                        var values = subColumn.SelectMany(x => x.Notes.Values).Distinct();
                        foreach (var value in values)
                        {
                            // if value is not found in any subcolumn
                            if (columnCheckCells.Any(x => x.Notes.Values.Contains(value)) == false)
                            {
                                if (blockCheckCells.Any(x => x.Notes.Values.Contains(value)))
                                {
                                    blockCheckCells.ForEach(x => x.Notes.Remove(value));
                                    changed = true;
                                }
                            }
                            // if value is not found in any subrow in block
                            if (blockCheckCells.Any(x => x.Notes.Values.Contains(value)) == false)
                            {
                                if (columnCheckCells.Any(x => x.Notes.Values.Contains(value)))
                                {
                                    columnCheckCells.ForEach(x => x.Notes.Remove(value));
                                    changed = true;
                                }
                            }
                        }
                    }
                }
            }
            return changed;
        }
    }
}
