using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleSudoku.Game
{
    public class CellNote
    {
        private int BitField { get; set; }

        public int[] Values => Enumerable.Range(1, 9)
            .Where(x => (BitField & (1 << x - 1)) != 0
            )
            .ToArray();

        public void Add(int value)
        {
            BitField |= 1 << value - 1;
        }

        public void Remove(int value)
        {
            BitField &= ~(1 << value - 1);
        }

        public void Clear()
        {
            BitField = 0;
        }

        public void SetAll()
        {
            BitField = ~(1 << 10);
        }
    }
}
