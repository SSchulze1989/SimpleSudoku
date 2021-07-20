using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSudoku.Game
{
    public class SudokuCell
    {
        public int Row { get; }
        public int Column { get; }
        public int? Value { get; set; }
        public int TrueValue { get; private set; }
        public CellNote Notes { get; }

        internal SudokuCell(int row, int col)
        {
            Row = row;
            Column = col;
            Value = null;
            Notes = new CellNote();
        }

        internal SudokuCell(int row, int col, int? value) : this(row, col)
        {
            Value = value;
        }

        internal void SetTrueValue(int value)
        {
            TrueValue = value;
        }

        public int GetIndex()
        {
            return Row * 9 + Column;
        }

        public override string ToString()
        {
            return $"Cell[{Row},{Column}] = {Value}";
        }

        public virtual SudokuCell Copy()
        {
            var newCell = new SudokuCell(Row, Column, Value);
            newCell.SetTrueValue(TrueValue);
            foreach(var value in Notes.Values)
            {
                newCell.Notes.Add(value);
            }
            return newCell;
        }
    }
}
