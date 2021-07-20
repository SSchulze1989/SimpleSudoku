using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SimpleSudoku.Game
{
    public class SudokuField
    {
        public SudokuCell[,] Cells { get; private set; }

        public bool Initialized { get; private set; }

        public SudokuCell[][] Rows => Cells.GetRows();
        public SudokuCell[][] Columns => Cells.GetColumns();
        public SudokuCell[][,] Blocks => Enumerable.Range(0, 9)
            .Select(x => GetBlock(x))
            .ToArray();

        public SudokuField()
        {
            Cells = new SudokuCell[9, 9];
            ResetCells();
        }

        public SudokuField(SudokuCell[,] cells)
        {
            Cells = cells;
        }

        public SudokuCell[] GetRow(int index) => Cells.GetRow(index);
        public SudokuCell[] GetColumn(int index) => Cells.GetColumn(index);

        public SudokuCell[] GetSubRow(int rowIndex, int blockIndex) => Cells.GetRow(rowIndex, Enumerable.Range(blockIndex * 3, 3).ToArray());
        public SudokuCell[] GetSubColumn(int columnIndex, int blockIndex) => Cells.GetColumn(columnIndex, Enumerable.Range(blockIndex * 3, 3).ToArray());

        public SudokuCell[,] GetBlock(int index)
        {
            var vIndex = index / 3;
            var hIndex = (index - vIndex * 3) % 3;
            return GetBlock(hIndex, vIndex);
        }
        public SudokuCell[,] GetBlock(int vIndex, int hIndex)
        {
            var block = new SudokuCell[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    block[i, j] = Cells[i + vIndex * 3, j + hIndex * 3];
                }
            }
            return block;
        }

        public void ResetCells()
        {
            for (int row = 0; row < Cells.GetLength(0); row++)
            {
                for (int col = 0; col < Cells.GetLength(1); col++)
                {
                    Cells[row, col] = new SudokuCell(row, col);
                }
            }
            Initialized = false;
        }

        internal void SetInitialized()
        {
            Initialized = true;
        }
    }
}
