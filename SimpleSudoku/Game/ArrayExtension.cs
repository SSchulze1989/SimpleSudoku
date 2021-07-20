using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleSudoku.Game
{
    public static class ArrayExtension
    {
        public static T[] GetRow<T>(this T[,] array, int rowIndex)
        {
            return Enumerable.Range(0, array.GetLength(1))
                .Select(x => array[rowIndex, x])
                .ToArray();
        }

        public static T[] GetRow<T>(this T[,] array, int rowIndex, int[] indexes)
        {
            return indexes
                .Select(x => array[rowIndex, x])
                .ToArray();
        }

        public static T[][] GetRows<T>(this T[,] array)
        {
            return Enumerable.Range(0, array.GetLength(0))
                .Select(x => array.GetRow(x))
                .ToArray();
        }

        public static T[] GetColumn<T>(this T[,] array, int columnIndex)
        {
            return Enumerable.Range(0, array.GetLength(0))
                .Select(x => array[x, columnIndex])
                .ToArray();
        }

        public static T[] GetColumn<T>(this T[,] array, int columnIndex, int[] indexes)
        {
            return indexes
                .Select(x => array[x, columnIndex])
                .ToArray();
        }

        public static T[][] GetColumns<T>(this T[,] array)
        {
            return Enumerable.Range(0, array.GetLength(1))
                .Select(x => array.GetColumn(x))
                .ToArray();
        }

        public static T[] To1DArray<T>(this T[,] array)
        {
            var values = new List<T>();
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    values.Add(array[i, j]);
                }
            }
            return values.ToArray();
        }
    }
}
