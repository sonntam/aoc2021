using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2021
{
    class Day9 : IAoCProgram
    {
        public Day9() : base(SameInputForBAsForA: true) { }

        private List<(int,int)> FindLowPoints( int[,] map)
        {
            int n = map.GetLength(0), m = map.GetLength(1);
            var lowPoints = new List<(int,int)>();

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    bool lowPoint = true;
                    var el = map[i, j];

                    if (i > 0 && el >= map[i - 1, j]) lowPoint = false;
                    if (i + 1 < n && el >= map[i + 1, j]) lowPoint = false;
                    if (j > 0 && el >= map[i, j - 1]) lowPoint = false;
                    if (j + 1 < m && el >= map[i, j + 1]) lowPoint = false;

                    if (lowPoint) lowPoints.Add((i,j));
                }
            }

            return lowPoints;
        }

        private int GetBasinSizeForLowPoint(int[,] map, (int, int) lowPointCoord, bool[,] doneMap = null)
        {
            int size = 1;
            if (doneMap == null)
            {
                doneMap = new bool[map.GetLength(0), map.GetLength(1)];
                doneMap.Fill2DArray(false);
            }

            doneMap[lowPointCoord.Item1, lowPointCoord.Item2] = true;

            var newCoords = GetAdjacentBasinLocations(map, lowPointCoord);

            foreach (var coord in newCoords)
            {
                if( doneMap[coord.Item1, coord.Item2] == false )
                    size += GetBasinSizeForLowPoint(map, coord, doneMap);
            }

            return size;
        }

        private List<(int,int)> GetAdjacentBasinLocations( int[,] map, (int,int) coordinate)
        {
            var adjacent = new List<(int, int)>();
            int n = map.GetLength(0), m = map.GetLength(1);
            int i = coordinate.Item1, j = coordinate.Item2;

            int el = map[i, j];

            if (i > 0 && el < map[i - 1, j] && map[i - 1, j] != 9) adjacent.Add((i - 1, j));
            if (i + 1 < n && el < map[i + 1, j] && map[i + 1, j] != 9) adjacent.Add((i + 1, j));
            if (j > 0 && el < map[i, j - 1] && map[i, j - 1] != 9) adjacent.Add((i, j - 1));
            if (j + 1 < m && el < map[i, j + 1] && map[i, j + 1] != 9) adjacent.Add((i, j + 1));

            return adjacent;
        }

        private List<int> FindLowPointValues( int[,] map ) {

            var lowPoints = FindLowPoints(map);

            return lowPoints.Select(x => map[x.Item1, x.Item2]).ToList();
        }

        public override string RunA(StreamReader input)
        {
            var mapList = new List<List<int>>();

            // Build map
            while(!input.EndOfStream)
            {
                var line = input.ReadLine().Select(x => int.Parse($"{x}")).ToList();
                mapList.Add(line);
            }

            var map = mapList.To2DArray();
            return FindLowPointValues(map).Select(x => x + 1).Sum().ToString();
        }

        public override string RunB(StreamReader input)
        {
            var mapList = new List<List<int>>();

            // Build map
            while (!input.EndOfStream)
            {
                var line = input.ReadLine().Select(x => int.Parse($"{x}")).ToList();
                mapList.Add(line);
            }

            var map = mapList.To2DArray();

            var lowPoints = FindLowPoints(map);

            var basinSizes = lowPoints.Select(x => GetBasinSizeForLowPoint(map, x)).OrderByDescending(x => x).ToList();

            return basinSizes.Take(3).Aggregate(1, (prod, el) => prod * el).ToString();
        }
    }
}
