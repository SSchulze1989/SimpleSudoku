using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleSudoku.Game
{
    /// <summary>
    /// Class to check if a Sudoku is valid. Outputs any invalid combination in ValidationFails property
    /// </summary>
    public class SudokuValidator
    {
        private SudokuField Field { get; }
        private List<ValidationFail> FailsList { get; set; }

        public IEnumerable<ValidationFail> ValidationFails => FailsList.ToArray();
        public bool ValidationFailed => ValidationFails?.Count() > 0 ? true : false;

        public SudokuValidator(SudokuField field)
        {
            Field = field;
        }

        private bool ValidateRow(SudokuCell[] row)
        {
            int bitField = 0b0;
            bool foundDuplicate = false;

            foreach(var cell in row.Where(x => x.Value != null))
            {
                var valueBit = 1 << (cell.Value.Value - 1) ;
                if ((bitField & valueBit) == 1)
                {
                    foundDuplicate = true;
                    break;
                }
                bitField |= valueBit;
            }

            if (foundDuplicate)
            {
                SetRowValidationFails(row);
            }

            return !foundDuplicate;
        }

        private bool ValidateColumn(SudokuCell[] column)
        {
            int bitField = 0b0;
            bool foundDuplicate = false;

            foreach (var cell in column.Where(x => x.Value != null))
            {
                var valueBit = 1 << (cell.Value.Value - 1);
                if ((bitField & valueBit) == 1)
                {
                    foundDuplicate = true;
                    break;
                }
                bitField |= valueBit;
            }

            if (foundDuplicate)
            {
                SetColumnValidationFails(column);
            }

            return !foundDuplicate;
        }

        private bool ValidateBlock(SudokuCell[,] block)
        {
            int bitField = 0b0;
            bool foundDuplicate = false;

            foreach (var cell in block
                .To1DArray()
                .Where(x => x.Value != null))
            {
                var valueBit = 1 << (cell.Value.Value - 1);
                if ((bitField & valueBit) == 1)
                {
                    foundDuplicate = true;
                    break;
                }
                bitField |= valueBit;
            }

            if (foundDuplicate)
            {
                SetBlockValidationFails(block);
            }

            return !foundDuplicate;
        }

        private void SetRowValidationFails(SudokuCell[] row)
        {
            for (int value = 1; value <= 9; value++)
            {
                var valueCells = row.Where(x => x.Value == value);
                if (valueCells.Count() > 1)
                {
                    FailsList.Add(new ValidationFail(valueCells.ToArray(), ValidationFailType.Row));
                }
            }
        }

        private void SetColumnValidationFails(SudokuCell[] column)
        {
            for (int value = 1; value <= 9; value++)
            {
                var valueCells = column.Where(x => x.Value == value);
                if (valueCells.Count() > 1)
                {
                    FailsList.Add(new ValidationFail(valueCells.ToArray(), ValidationFailType.Column));
                }
            }
        }

        private void SetBlockValidationFails(SudokuCell[,] block)
        {
            for (int value = 1; value <= 9; value++)
            {
                var valueCells = block
                    .To1DArray()
                    .Where(x => x.Value == value);
                if (valueCells.Count() > 1)
                {
                    FailsList.Add(new ValidationFail(valueCells.ToArray(), ValidationFailType.Block));
                }
            }
        }

        public bool ValidateAll()
        {
            var fails = new List<ValidationFail>();
            for (int value = 1; value <= 9; value++)
            {
                // Chek for duplicates in rows
                foreach (var row in Field.Rows)
                {
                    var valueCells = row.Where(x => x.Value == value);
                    if (valueCells.Count() > 1)
                    {
                        fails.Add(new ValidationFail(valueCells.ToArray(), ValidationFailType.Row));
                    }
                }

                foreach (var column in Field.Columns)
                {
                    var valueCells = column.Where(x => x.Value == value);
                    if (valueCells.Count() > 1)
                    {
                        fails.Add(new ValidationFail(valueCells.ToArray(), ValidationFailType.Column));
                    }
                }

                foreach (var block in Field.Blocks)
                {
                    var valueCells = block.To1DArray()
                        .Where(x => x.Value == value);
                    if (valueCells.Count() > 1)
                    {
                        fails.Add(new ValidationFail(valueCells.ToArray(), ValidationFailType.Block));
                    }
                }
            }

            FailsList = fails;

            return FailsList.Count == 0;
        }

        public bool ValidateCell(SudokuCell cell)
        {
            bool result = true;
            // Reset validation fails
            FailsList = new List<ValidationFail>();
            
            result &= ValidateRow(Field.GetRow(cell.Row));
            result &= ValidateColumn(Field.GetColumn(cell.Column));
            result &= ValidateBlock(Field.GetBlock(cell.Row / 3, cell.Column / 3));

            return result;
        }
    }
}
