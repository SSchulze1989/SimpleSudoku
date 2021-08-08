using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleSudoku.Game
{
    public class ValidationFail
    {
        public SudokuCell[] FailedCells { get; }
        public ValidationFailType FailType { get; }

        public ValidationFail(SudokuCell[] cells, ValidationFailType type)
        {
            FailedCells = cells;
            FailType = type;
        }

        public override string ToString()
        {
            return FailType.ToString() + string.Join(", ", FailedCells.Select(x => $"C({x.Row},{x.Column})"));
        }
    }

    public enum ValidationFailType
    {
        Row,
        Column,
        Block
    }
}
