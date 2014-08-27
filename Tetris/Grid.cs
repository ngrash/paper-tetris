using System.Text;

namespace Tetris
{
    internal class Grid<T>
    {
        private readonly T[,] _data;

        public Grid(T[,] data)
        {
            _data = data;
        }

        public Grid(int width, int height)
        {
            _data = new T[height, width];
        }

        public int Width
        {
            get
            {
                return _data.GetLength(1);
            }
        }

        public int Height
        {
            get
            {
                return _data.GetLength(0);
            }
        }

        public T this[int x, int y]
        {
            get
            {
                return _data[y, x];
            }

            set
            {
                _data[y, x] = value;
            }
        }

        public string Inspect
        {
            get
            {
                var builder = new StringBuilder();

                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        builder.Append(_data[y, x]);
                    }

                    builder.AppendLine();
                }

                return builder.ToString();
            }
        }
    }
}
