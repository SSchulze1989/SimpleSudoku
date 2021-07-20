using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleSudoku.Game
{
    public class SudokuSolver
    {
        private SudokuField SolverField { get; set; }

        public SudokuSolver(SudokuField field)
        {
            SolverField = new SudokuField(field);
            foreach (var cell in SolverField.Cells.To1DArray())
            {
                cell.Notes.SetAll();
            }
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

        public void Solve()
        {
            GetPossibleValues1();
            GetPossibleValues2();
            GetPossibleValues3();
        }


        /// <summary>
        /// Exclude possible values on first sight using the existing Values
        /// </summary>
        private void GetPossibleValues1()
        {
            // iterate over each cell that has a value
            foreach (var cell in SolverField.Cells.To1DArray().Where(x => x.Value != null))
            {
                cell.Notes.Clear();
                SetNotesForCellValue(cell, cell.Value.Value);   
            }
        }

        /// <summary>
        /// Exclude possible by finding unique missing numbers in row/column/block
        /// </summary>
        private void GetPossibleValues2()
        {
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
                    if (uniqueCell != null)
                    {
                        uniqueCell.Notes.Clear();
                        uniqueCell.Notes.Add(uniqueValue);
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
                    if (uniqueCell != null)
                    {
                        uniqueCell.Notes.Clear();
                        uniqueCell.Notes.Add(uniqueValue);
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
                    if (uniqueCell != null)
                    {
                        uniqueCell.Notes.Clear();
                        uniqueCell.Notes.Add(uniqueValue);
                    }
                }
            }
        }

        /// <summary>
        /// Exclude possible values by looking at the sub rows/columns of a block and the respective rows/columns
        /// </summary>
        private void GetPossibleValues3()
        {
            // iterate over each block and then over each sub row/column
            for (int vIndex = 0; vIndex < 3; vIndex++)
            {
                for (int hIndex = 0; hIndex < 3; hIndex++)
                {
                    var block = SolverField.GetBlock(vIndex, hIndex);

                    // iterate over rows
                    foreach(var (subRow, index) in block.GetRows().WithIndex())
                    {
                        var checkSubRows = Enumerable.Range(0, 3)
                            .Where(x => x != vIndex)
                            .Select(x => SolverField.GetSubRow(index, x));
                        // only check cells without value
                        var checkCells = checkSubRows.SelectMany(x => x).Where(x => x.Value == null);

                        var excludeSubRows = Enumerable.Range(0, 3)
                            .Where(x => x != index)
                            .Select(x => block.GetRow(x));
                        // only exclude cells without value
                        var excludeCells = excludeSubRows.SelectMany(x => x).Where(x => x.Value == null);

                        // Check for values that appear in selected subrow but not in checkSubrows
                        var values = subRow.SelectMany(x => x.Notes.Values).Distinct();
                        foreach(var value in values)
                        {
                            // if value is not found in any subrow
                            if (checkCells.Any(x => x.Notes.Values.Contains(value) == false))
                            {
                                excludeCells.ForEach(x => x.Notes.Remove(value));
                            }
                        }
                    }

                    foreach (var (subColumn, index) in block.GetColumns().WithIndex())
                    {
                        // get subcolumns to compare with => 2 remaining subcolumns of this column
                        var checkSubColumns = Enumerable.Range(0, 3)
                            .Where(x => x != hIndex)
                            .Select(x => SolverField.GetSubColumn(index, x));
                        // only check cells without value
                        var checkCells = checkSubColumns.SelectMany(x => x).Where(x => x.Value == null);

                        // get subcolumns that need to be set => 2 remaining subcolumns in this block
                        var excludeSubColumns = Enumerable.Range(0, 3)
                            .Where(x => x != index)
                            .Select(x => block.GetColumn(x));
                        // only exclude cells without value
                        var excludeCells = excludeSubColumns.SelectMany(x => x).Where(x => x.Value == null);

                        // Check for values that appear in selected subcolumn but not in checkSubcolumns
                        var values = subColumn.SelectMany(x => x.Notes.Values).Distinct();
                        foreach (var value in values)
                        {
                            // if value is not found in any subcolumn
                            if (checkCells.Any(x => x.Notes.Values.Contains(value) == false))
                            {
                                excludeCells.ForEach(x => x.Notes.Remove(value));
                            }
                        }
                    }
                }
            }
        }
    }
}
