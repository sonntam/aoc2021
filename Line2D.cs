using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2021
{
    class Line2D
    {
        public Point P1;
        public Point P2;

        public Line2D( Point P1, Point P2 )
        {
            this.P1 = P1;
            this.P2 = P2;
        }

        public bool IsHorizontal => P1.Y == P2.Y;
        public bool IsVertical => P1.X == P2.X;
        public bool IsStraight => IsVertical || IsHorizontal;

        public bool IsDiagonal => Math.Abs(P1.X - P2.X) == Math.Abs(P1.Y - P2.Y);
    }
}
