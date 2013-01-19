using System.Collections;
using System.Collections.Generic;

namespace Hacking
{
    public class RotateableGrid<T> : IEnumerable
    {
        private T[,] members;

        private int width;
        private int height;

        public RotateableGrid(int width_, int height_)
        {
            width = width_;
            height = height_;
            members = new T[Width, Height];
        }

        public int Width { get { return width; } }
        public int Height { get { return height; } }
        public T this[int x, int y] { get { return members[x, y]; } set { members[x, y] = value; } }

        public void rotate()
        {
            // Mhrm. Hardcoding the 'direction' of the rotation to just the
            // one we need sucks in class as generic as this. But implementing
            // them in all directions would be a useless effort.
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y + 1 < Height; y++)
                {
                    Utils.Swap(ref members[x, y], ref members[x, y + 1]);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return members.GetEnumerator();
        }
    }
}
