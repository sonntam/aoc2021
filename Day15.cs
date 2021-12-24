using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2021
{
    class Day15 : IAoCProgram
    {
        public Day15() : base(SameInputForBAsForA: true) { }

        public enum Direction
        {
            UP,
            DOWN,
            LEFT,
            RIGHT,
            UNKNOWN
        }

        public override string RunA(StreamReader input)
        {
            List<List<int>> riskLines = new List<List<int>>();
            while (!input.EndOfStream)
            {
                riskLines.Add(input.ReadLine().Select(x => int.Parse($"{x}")).ToList());
            }

            var map = riskLines.To2DArray();
            var n = map.GetLength(0); var m = map.GetLength(1);

            // Dijkstra's algorithm
            Point2D startpos = new Point2D((int)0, (int)0);
            Point2D endpos = new Point2D(m - 1, n - 1);
            startpos = PathFind(map, out int[,] optimalCostToGo, out bool[,] done, startpos, endpos);

            return optimalCostToGo[endpos.Y, endpos.X].ToString();
        }

        private static Point2D PathFind(int[,] map, out int[,] optimalCostToGo, out bool[,] done, Point2D startpos, Point2D endpos)
        {
            var n = map.GetLength(0); var m = map.GetLength(1);
            var coordsMatrix = Enumerable.Range(0, n * m).Select(c => new Point2D(c / n, c % n)).ToMatrixColMajor(n, m);

            var coordDone = new bool[n, m];
            coordDone.Fill2DArray(false);

            optimalCostToGo = new int[n, m];
            optimalCostToGo.Fill2DArray(int.MaxValue);
            optimalCostToGo[startpos.Y, startpos.X] = 0;

            while (true)
            {
                var startcost = optimalCostToGo[startpos.Y, startpos.X];
                var neighbors = new List<Point2D>() {
                    new Point2D(startpos.X, startpos.Y - 1),
                    new Point2D(startpos.X, startpos.Y + 1),
                    new Point2D(startpos.X- 1, startpos.Y),
                    new Point2D(startpos.X + 1, startpos.Y),
                };

                neighbors = neighbors.Where(c => !(c.X < 0 || c.Y < 0 || c.X >= m || c.Y >= n) && !coordDone[c.Y, c.X]).ToList();

                // Check not done neighbours
                foreach (var neighbor in neighbors)
                {
                    optimalCostToGo[neighbor.Y, neighbor.X] = Math.Min(optimalCostToGo[neighbor.Y, neighbor.X], startcost + map[neighbor.Y, neighbor.X]);
                }
                coordDone[startpos.Y, startpos.X] = true;

                if (startpos == endpos)
                    break;

                // zip opt2go & done
                var optDone = optimalCostToGo.ToEnumerableColMajor().Zip(coordDone.ToEnumerableColMajor());

                // no neighbors? infeasible!
                if (neighbors.Count() == 0 && optDone.Where(x => !x.Second).All(x => x.First == int.MaxValue))
                    throw new Exception("Infeasible path finding problem!");



                // Select next
                startpos = coordsMatrix.ToEnumerableColMajor()
                    .Zip(optDone)
                    .Where(e => !e.Second.Second && e.Second.First != int.MaxValue)
                    .MinBy(e => e.Second.First)
                    .First;
            }

            done = coordDone;

            return startpos;
        }

        public override string RunB(StreamReader input)
        {
            List<List<int>> riskLines = new List<List<int>>();
            while (!input.EndOfStream)
            {
                riskLines.Add(input.ReadLine().Select(x => int.Parse($"{x}")).ToList());
            }

            var mapTile = riskLines.To2DArray();
            int n0 = mapTile.GetLength(0), m0 = mapTile.GetLength(1);
            var mapField = mapTile.Resize(n0 * 5, m0 * 5);

            // Repeat map 5 times in both directions
            for( int i = 1; i <= (5-1)*2; i++ )
            {
                var tempTile = mapTile.ToEnumerableColMajor().Select(x => ((x - 1 + i) % 9) + 1).ToMatrixColMajor(n0,m0);

                // Insert
                var insertPositions = Enumerable.Range(0, i + 1)
                    .Zip(Enumerable.Range(0, i + 1).Reverse())
                    .Select(x => new Point2D(x.First, x.Second))
                    .Where(c => c.X < 5 && c.Y < 5);

                foreach( var insertPosition in insertPositions )
                {
                    mapField.Insert(tempTile, (int)insertPosition.Y * n0, (int)insertPosition.X * m0);
                }
            }

            // Dijkstra's algorithm
            var n = mapField.GetLength(0); var m = mapField.GetLength(1);
            Point2D startpos = new Point2D((int)0, (int)0);
            Point2D endpos = new Point2D(m - 1, n - 1);
            startpos = PathFind(mapField, out int[,] optimalCostToGo, out bool[,] done, startpos, endpos);

            return optimalCostToGo[endpos.Y, endpos.X].ToString();
        }
    }
}
