using System.Collections;
using System.Collections.Generic;

namespace Hacking
{
    public class RotateableGrid<T> : IEnumerable, IEnumerable<T>
    {
        private Row[] rows;

        public int Columns { get { return rows[0].Count; } }
        public int Rows { get { return rows.Length; } }

        public Row this[int i] { get { return rows[i]; } }
        public T this[int x, int y] { get { return rows[x][y]; } set { rows[x][y] = value; } }

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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
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

        public class Enumerator : IEnumerator, IEnumerator<T>
        {
            private int position = -1;
            private RotateableGrid<T> grid;

            object IEnumerator.Current { get { return current(); } }
            T IEnumerator<T>.Current { get { return current(); } }

            public Enumerator(RotateableGrid<T> enumeratable)
            {
                grid = enumeratable;
            }

            public bool MoveNext()
            {
                if (position <= grid.Columns * grid.Rows)
                {
                    position += 1;
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                position = -1;
            }

            public void Dispose() {}

            private T current()
            {
                // TODO: This implementation sucks, the one below should be
                //       used -- but I'm too matschig in my head to get it
                //       working right now o.O
                for (int i = 0; i < grid.Rows; i++)
                {
                    for (int j = 0; j < grid.Columns; j++)
                    {
                        if (i * grid.Columns + j == position)
                        {
                            return grid[i, j];
                        }
                    }
                }
                return grid[0, 0];

                //int x = position / grid.Rows;
                //int y = position % grid.Columns;
                //return grid[x, y];
            }
        }
    }
}
