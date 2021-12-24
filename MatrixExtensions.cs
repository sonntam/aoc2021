using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2021
{
    public static class MatrixExtensions
    {
        public static int[,] AppendRight(this int[,] map, int[,] addmap)
        {
            if (addmap.GetLength(0) != map.GetLength(0)) throw new ArgumentException($"Appended map should have row count {map.GetLength(0)}");

            int n = map.GetLength(0);
            int m = map.GetLength(1) + addmap.GetLength(1);

            return map.ToEnumerableColMajor().Concat(addmap.ToEnumerableColMajor()).ToMatrixColMajor(n, m);
        }

        public static int[,] AppendDown(this int[,] map, int[,] addmap)
        {
            if (addmap.GetLength(1) != map.GetLength(1)) throw new ArgumentException($"Appended map should have col count {map.GetLength(1)}");

            int n = map.GetLength(0) + addmap.GetLength(0);
            int m = map.GetLength(1);

            return map.ToEnumerableRowMajor().Concat(addmap.ToEnumerableRowMajor()).ToMatrixRowMajor(n, m);
        }

        public static void Insert(this int[,] map, int[,] insertmap, int i0, int j0)
        {
            if (map.GetLength(0) < insertmap.GetLength(0) + i0) throw new ArgumentException("Inserted matrix is too long");
            if (map.GetLength(1) < insertmap.GetLength(1) + j0) throw new ArgumentException("Inserted matrix is too wide");

            int n0 = insertmap.GetLength(0);
            int m0 = insertmap.GetLength(1);

            for( int i = 0; i < n0; i++ )
            {
                for (int j = 0; j < m0; j++)
                {
                    map[i+i0, j+j0] = insertmap[i, j];
                }
            }
        }

        public static int[,] Resize(this int[,] map, int n0, int m0, int fillval = default)
        {
            var outmap = new int[n0, m0];
            int n = map.GetLength(0), m = map.GetLength(1);

            for (int i = 0; i < n0; i++)
            {
                for (int j = 0; j < m0; j++)
                {
                    outmap[i, j] = (i < n && j < m) ? map[i, j] : fillval;
                }
            }

            return outmap;
        }

        public static void Add(this int[,] map, int val )
        {
            int n = map.GetLength(0), m = map.GetLength(1);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    map[i, j] += val;
                }
            }
        }

        public static void ToPlainString(this int[,] map, int? highlightvalue = null)
        {
            int n = map.GetLength(0), m = map.GetLength(1);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if( highlightvalue.HasValue && map[i,j] == highlightvalue.Value )
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    Console.Write($"{map[i, j]}");
                    if (highlightvalue.HasValue && map[i, j] == highlightvalue.Value)
                    {
                        Console.ResetColor();
                    }
                }
                Console.WriteLine("");
            }
        }

        public static void ToConsoleInsert(this int[,] map, int? highlightvalue = null)
        {
            int n = map.GetLength(0), m = map.GetLength(1);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (highlightvalue.HasValue && map[i, j] == highlightvalue.Value)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    Console.Write($"{map[i, j]}");
                    if (highlightvalue.HasValue && map[i, j] == highlightvalue.Value)
                    {
                        Console.ResetColor();
                    }
                }
                Console.CursorLeft -= m;
                Console.CursorTop++;
            }
        }

        public static T[,] To2DArray<T>(this List<List<T>> map)
        {
            if ((map == null) || (map.Any(subList => subList.Any() == false)))
                throw new ArgumentException("Input list is not properly formatted with valid data");

            int index = 0;
            int subindex;

            return

               map.Aggregate(new T[map.Count(), map.Max(sub => sub.Count())],
                             (array, subList) =>
                             {
                                 subindex = 0;
                                 subList.ForEach(itm => array[index, subindex++] = itm);
                                 ++index;
                                 return array;
                             });
        }

        
        public static IEnumerable<T> ToEnumerableRowMajor<T>(this T[,] matrix)
        {
            int n = matrix.GetLength(0);
            int m = matrix.GetLength(1);

            for( int i = 0; i < n; i++ )
            {
                for( int j = 0; j < m; j++ )
                {
                    yield return matrix[i, j];
                }
            }
        }

        public static IEnumerable<T> ToEnumerableColMajor<T>(this T[,] matrix)
        {
            int n = matrix.GetLength(0);
            int m = matrix.GetLength(1);

            for (int j = 0; j < m; j++)
            {
                for (int i = 0; i < n; i++)
                {
                    yield return matrix[i, j];
                }
            }
        }

        public static void Fill2DArray<T>(this T[,] arr, T value)
        {
            int n = arr.GetLength(0);
            int m = arr.GetLength(1);

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < m; ++j)
                {
                    arr[i, j] = value;
                }
            }
        }

        public static void Fill2DArrayWithNew<T>(this T[,] arr) where T : new()
        {
            int n = arr.GetLength(0);
            int m = arr.GetLength(1);

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < m; ++j)
                {
                    arr[i, j] = new T();
                }
            }
        }
    }
}
