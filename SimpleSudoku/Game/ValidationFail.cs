using System;
using System.Collections.Generic;
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
    }

    public enum ValidationFailType
    {
        Row,
        Column,
        Block
    }
}
