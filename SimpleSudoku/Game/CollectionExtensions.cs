using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleSudoku.Game
{
    public static class CollectionExtensions
    {
        public static ICollection<T> RemoveMany<T>(this ICollection<T> collection, IEnumerable<T> values)
        {
            foreach(var value in values)
            {
                collection.Remove(value);
            }
            return collection;
        }

        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
        {
            return source.Select((item, index) => (item, index));
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach(var item in source)
            {
                action?.Invoke(item);
            }
        }
    }
}
