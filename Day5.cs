using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2021
{
    
    class Day5 : IAoCProgram
    {

        public Day5() : base(SameInputForBAsForA: true) { }

        public override string RunA(StreamReader input)
        {

            List<Line2D> lines = new List<Line2D>();

            while(!input.EndOfStream)
            {
                var lineStr = input.ReadLine();
                var linePoints = lineStr.Split(" -> ").Select( x => x.Split(',').Select( y => int.Parse(y)).ToArray()).ToArray();

                lines.Add( new Line2D(new Point(linePoints[0][0], linePoints[0][1]), new Point(linePoints[1][0], linePoints[1][1])) );
            }

            // draw map
            SparseMatrix<int> map = new SparseMatrix<int>(0, 0, 0);

            foreach( var line in lines )
            {
                if (!line.IsStraight) continue;

                Func<Line2D, bool, (int, int)> CoordGetter = (Line2D l, bool getX) =>
                   {
                       return getX ? (Math.Min(l.P1.X, l.P2.X), Math.Max(l.P1.X, l.P2.X)) : (Math.Min(l.P1.Y, l.P2.Y), Math.Max(l.P1.Y, l.P2.Y));
                   };


                if (line.IsHorizontal)
                {
                    for (int i = Math.Min(line.P1.X, line.P2.X); i < Math.Max(line.P1.X, line.P2.X) + 1; i++)
                    {
                        map[line.P1.Y,i]++;
                    }
                }

                if( line.IsVertical)
                {
                    for (int i = Math.Min(line.P1.Y, line.P2.Y); i < Math.Max(line.P1.Y, line.P2.Y) + 1; i++)
                    {
                        map[i, line.P1.X]++;
                    }
                }
            }

            return map.Data.Values.Select(x => x.Values.Where( x => x > 1).Select( _ => 1).Sum()).Sum().ToString();
        }

        public override string RunB(StreamReader input)
        {

            List<Line2D> lines = new List<Line2D>();

            while (!input.EndOfStream)
            {
                var lineStr = input.ReadLine();
                var linePoints = lineStr.Split(" -> ").Select(x => x.Split(',').Select(y => int.Parse(y)).ToArray()).ToArray();

                lines.Add(new Line2D(new Point(linePoints[0][0], linePoints[0][1]), new Point(linePoints[1][0], linePoints[1][1])));
            }

            // draw map
            SparseMatrix<int> map = new SparseMatrix<int>(0, 0, 0);

            foreach (var line in lines)
            {

                if (line.IsHorizontal)
                {
                    for (int i = Math.Min(line.P1.X, line.P2.X); i < Math.Max(line.P1.X, line.P2.X) + 1; i++)
                    {
                        map[line.P1.Y, i]++;
                    }
                }

                if (line.IsVertical)
                {
                    for (int i = Math.Min(line.P1.Y, line.P2.Y); i < Math.Max(line.P1.Y, line.P2.Y) + 1; i++)
                    {
                        map[i, line.P1.X]++;
                    }
                }

                if( line.IsDiagonal)
                {
                    int dx = line.P2.X - line.P1.X; dx = dx / Math.Abs(dx);
                    int dy = line.P2.Y - line.P1.Y; dy = dy / Math.Abs(dy);

                    int x = line.P1.X;
                    int y = line.P1.Y;
                    while(true)
                    {
                        map[y, x]++;

                        if (x == line.P2.X || y == line.P2.Y) break;
                        x += dx;
                        y += dy;
                    }
                }
            }

            return map.Data.Values.Select(x => x.Values.Where(x => x > 1).Select(_ => 1).Sum()).Sum().ToString();
        }
    }
}
