using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSudoku.Game
{
    public static class ICollectionExtensions
    {
        public static ICollection<T> RemoveMany<T>(this ICollection<T> collection, IEnumerable<T> values)
        {
            foreach(var value in values)
            {
                collection.Remove(value);
            }
            return collection;
        }
    }
}
