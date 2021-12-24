using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2021
{
    class Day17 : IAoCProgram
    {
        public class TargetArea
        {
            public Point2D UpperLeft { get; protected set; }
            public Point2D LowerRight { get; protected set; }

            public TargetArea(Point2D upperLeft, Point2D lowerRight)
            {
                UpperLeft = upperLeft;
                LowerRight = lowerRight;
            }

            public bool Contains(Point2D coord)
            {
                if (coord.X < UpperLeft.X || coord.X > LowerRight.X) return false;
                if (coord.Y > UpperLeft.Y || coord.Y < LowerRight.Y) return false;
                return true;
            }
        }
        
        public Day17() : base(SameInputForBAsForA: true) { }

        public override string RunA(StreamReader input)
        {
            // Read target area
            var lineVal = input.ReadLine();
            var coordParts = lineVal.Substring(lineVal.IndexOf("x=")).Split(',').Select(x => x.Trim());
            var coords = coordParts.Select(x => x.Substring(x.IndexOf('=') + 1).Split("..").Select( x => int.Parse(x)).ToArray()).ToArray();

            var rect = new TargetArea(new Point2D(coords[0][0], coords[1][1]), new Point2D(coords[0][1], coords[1][0]));

            // Highest Y position
            var maxYVelocity = GetMaxInitVelocityY(rect);

            var maxYPos = Enumerable.Range(1, maxYVelocity).Sum();

            return maxYPos.ToString();
        }

        public override string RunB(StreamReader input)
        {
            // Read target area
            var lineVal = input.ReadLine();
            var coordParts = lineVal.Substring(lineVal.IndexOf("x=")).Split(',').Select(x => x.Trim());
            var coords = coordParts.Select(x => x.Substring(x.IndexOf('=') + 1).Split("..").Select(x => int.Parse(x)).ToArray()).ToArray();

            var rect = new TargetArea(new Point2D(coords[0][0], coords[1][1]), new Point2D(coords[0][1], coords[1][0]));

            // Brute da force
            var minX = GetMinInitVelocityX(rect);
            var maxX = GetMaxInitVelocityX(rect);
            var minY = GetMinInitVelocityY(rect);
            var maxY = GetMaxInitVelocityY(rect);

            int counter = 0;

            var ics = Enumerable.Range(minX, maxX - minX + 1).SelectMany(x => Enumerable.Range(minY, maxY - minY + 1).Select(y => new Point2D(x, y)));

            Console.WriteLine($"Number of combinations: {ics.Count()}");

            var count = ics.AsParallel().Where(ic => LandsInTargetArea(rect, ic)).Count();

            return count.ToString();
        }

        private bool LandsInTargetArea(TargetArea area, Point2D ic)
        {
            Point2D pos = new Point2D(0, 0);
            bool crossedFlag = false;

            while (true)
            {
                // Sim
                pos.X += ic.X;
                ic.X = ic.X + (ic.X > 0 ? -1 : (ic.X < 0 ? 1 : 0));

                pos.Y += ic.Y;
                ic.Y--;

                if (pos.Y < area.LowerRight.Y) break;
                if (area.Contains(pos)) return true;

            }

            return false;
        }

        private int GetMinInitVelocityX(TargetArea area)
        {
            var initVelX = 0;
            while(true)
            {
                if( Enumerable.Range(1, initVelX).Sum() >= area.UpperLeft.X )
                {
                    return initVelX;
                }
                initVelX++;
            }
        }

        private int GetMaxInitVelocityX(TargetArea area)
        {
            return area.LowerRight.X;
        }

        private int GetMaxInitVelocityY(TargetArea area)
        {
            return Math.Abs(area.LowerRight.Y + 1);
        }

        private int GetMinInitVelocityY(TargetArea area)
        {
            return area.LowerRight.Y;
        }

    }
}
