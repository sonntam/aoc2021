using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2021
{
    public static class EnumerableExtensions
    {
        public static T MinBy<T>(this IEnumerable<T> source, Func<T, IComparable> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            var min = source.First();
            var minval = selector(min);
            foreach (var element in source.Skip(1))
            {
                var curval = selector(element);
                if (minval.CompareTo(curval) > 0)
                {
                    min = element;
                    minval = curval;
                }
            }

            return min;
        }

        public static T MaxBy<T>(this IEnumerable<T> source, Func<T, IComparable> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            var max = source.First();
            var maxval = selector(max);
            foreach (var element in source.Skip(1))
            {
                var curval = selector(element);
                if (maxval.CompareTo(curval) < 0) {
                    max = element;
                    maxval = curval;
                }
            }

            return max;
        }

        public static T[,] ToMatrixRowMajor<T>(this IEnumerable<T> enumerable, int numRows, int numCols)
        {
            if (enumerable.Count() != numRows * numCols) throw new ArgumentException("Number of elements must fit matrix size");

            var ret = new T[numRows, numCols];

            int i = 0, j = 0, el = 0;
            foreach (var item in enumerable)
            {
                ret[i, j] = item;
                el++;
                j = el % numRows;
                i = el / numRows;
            }

            return ret;
        }

        public static T[,] ToMatrixColMajor<T>(this IEnumerable<T> enumerable, int numRows, int numCols)
        {
            if (enumerable.Count() != numRows * numCols) throw new ArgumentException("Number of elements must fit matrix size");

            var ret = new T[numRows, numCols];

            int i = 0, j = 0, el = 0;
            foreach (var item in enumerable)
            {
                ret[i, j] = item;
                el++;
                i = el % numRows;
                j = el / numRows;
            }

            return ret;
        }

        public static int BitsToInt(this IEnumerable<bool> bitstream)
        {
            var intVal = 0;
            foreach( var bit in bitstream )
            {
                intVal <<= 1;
                intVal += bit ? 1 : 0;
            }

            return intVal;
        }

        public static IEnumerable<T> IterateTreeBreadthFirst<T>(this T root, Func<T, IEnumerable<T>> childrenF)
        {
            var q = new List<T>() { root };
            while (q.Any())
            {
                var c = q[0];
                q.RemoveAt(0);
                q.AddRange(childrenF(c) ?? Enumerable.Empty<T>());
                yield return c;
            }
        }

        public static IEnumerable<T> IterateTreeDepthFirst<T>(this T root, Func<T, IEnumerable<T>> childrenF)
        {
            var q = new List<T>() { root };
            while (q.Any())
            {
                var c = q[0];
                q.RemoveAt(0);
                q.InsertRange(0, childrenF(c) ?? Enumerable.Empty<T>());
                yield return c;
            }
        }
    }
}
