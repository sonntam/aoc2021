using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2021
{
    public class SparseMatrix<T>
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public long Size => Width * Height;

        public T DefaultValue { get; private set; }

        private Dictionary<long, Dictionary<long, T>> _cells = new Dictionary<long, Dictionary<long, T>>();

        public Dictionary<long, Dictionary<long, T>> Data { get { return _cells; }  }

        public SparseMatrix(int w, int h, T defaultval)
        {
            this.Width = w;
            this.Height = h;
            this.DefaultValue = defaultval;
        }

        public T this[int row, int col]
        {
            get
            {
                if (row >= Height) Height = row+1;
                if (col >= Width) Width = col + 1;

                if (_cells.TryGetValue(row, out Dictionary<long, T> column))
                {
                    if( column.TryGetValue(col, out T result) )
                    {
                        return result;
                    }
                }
                return DefaultValue;
            }
            set
            {
                if( !_cells.ContainsKey(row) )
                {
                    _cells[row] = new Dictionary<long, T>();
                }

                _cells[row][col] = value;
            }
        }
    }
}
