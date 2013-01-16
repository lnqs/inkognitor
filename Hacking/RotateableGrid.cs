using System.Collections.Generic;

namespace Hacking
{
    public class RotateableGrid<T>
    {
        private Row[] rows;

        public int Columns { get { return rows[0].Count; } }
        public int Rows { get { return rows.Length; } }

        public Row this[int i] { get { return rows[i]; } }

        public RotateableGrid(int columnCount, int rowCount)
        {
            rows = new Row[rowCount];

            for (int i = 0; i < Rows; i++)
            {
                rows[i] = new Row(columnCount);
            }
        }

        public void rotate()
        {
            Row first = rows[0];
            for (int i = 1; i < Rows; i++)
            {
                rows[i - 1] = rows[i];
            }
            rows[Rows - 1] = first;
        }

        public class Row
        {
            private T[] members;
            private int count;

            public int Count { get { return count; } }
            public T this[int i] { get { return members[i]; } set { members[i] = value; } }

            public Row(int length)
            {
                count = length;
                members = new T[length];
            }
        }
    }
}
