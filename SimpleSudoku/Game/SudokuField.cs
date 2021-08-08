using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

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

        public SudokuField(SudokuField field)
        {
            Cells = new SudokuCell[field.Cells.GetLength(0), field.Cells.GetLength(1)];
            // copy cells from provided field
            for (int row = 0; row < field.Cells.GetLength(0); row++)
            {
                for (int col = 0; col < field.Cells.GetLength(1); col++)
                {
                    Cells[row, col] = field.Cells[row, col].Copy();
                }
            }
            Initialized = field.Initialized;
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
                    Cells[row, col].SetStartCell(false);
                }
            }
            Initialized = false;
        }

        internal void SetInitialized()
        {
            Initialized = true;
        }

        /// <summary>
        /// Encode sudoku state in a base64 string
        /// </summary>
        /// <returns></returns>
        public string Save()
        {
            var builder = new StringBuilder();
            foreach(var cell in Cells.To1DArray())
            {
                builder.AppendFormat("{0};{1};{2};{3}\n", cell.Value.GetValueOrDefault(), cell.TrueValue, System.Convert.ToInt32(cell.IsStartCell), cell.Notes.Values.Count() > 0 ? cell.Notes.Values.Select(x => x.ToString()).Aggregate((x, y) => $"{x},{y}") : "");
            }
            var bytes = System.Text.Encoding.UTF8.GetBytes(builder.ToString());
            var base64String = System.Convert.ToBase64String(bytes);

#if DEBUG
            Trace.WriteLine(base64String);
#endif

            return base64String;
        }

        /// <summary>
        /// Load playfield from a baset64 string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static SudokuField Load(string data)
        {
            var field = new SudokuField();
            field.ResetCells();
            var bytes = System.Convert.FromBase64String(data);
            var dataString = Encoding.UTF8.GetString(bytes);

            // get lines
            var dataLines = dataString
                .Split('\n')
                .Where(x => string.IsNullOrEmpty(x) == false);
            var cells1D = field.Cells.To1DArray();
            foreach(var (line, index) in dataLines.WithIndex())
            {
                var (value, trueValue, startCell, notes) = decodeDataString(line);
                var cell = cells1D[index];
                cell.Value = value;
                cell.SetTrueValue(trueValue);
                cell.Notes.Values = notes;
                cell.SetStartCell(startCell);
            }

            return field;
        }

        private static (int? value, int trueValue, bool startCell, int[] notes) decodeDataString(string data)
        {
            var parts = data.Split(';');

            int? value = System.Convert.ToInt32(parts[0]);
            int trueValue = System.Convert.ToInt32(parts[1]);

            if (value == 0)
            {
                value = null;
            }
            if (trueValue == 0)
            {
                throw new Exception("Error while decoding data string: True value was 0");
            }

            bool startCell = System.Convert.ToBoolean(System.Convert.ToInt32(parts[2]));

            var notes = parts[3]
                .Split(',')
                .Where(x => string.IsNullOrEmpty(x) == false);
            var noteValues = new List<int>();
            notes.ForEach(x => noteValues.Add(System.Convert.ToInt32(x)));

            return (value, trueValue, startCell, noteValues.ToArray());
        }
    }
}
