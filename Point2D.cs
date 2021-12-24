using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2021
{
    [DebuggerDisplay("x={X}, y={Y}")]
    public struct Point2D : IEquatable<Point2D>
    {
        public dynamic X;
        public dynamic Y;
        public Point2D(dynamic x, dynamic y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(Point2D l, Point2D r)
        {
            return l.Equals(r);
        }

        public static bool operator !=(Point2D l, Point2D r)
        {
            return !(l == r);
        }

        public bool Equals(Point2D other)
        {
            return other.X == X && other.Y == Y;
        }

        public Point2D MirrorX(dynamic x)
        {
            return new Point2D(x - (X - x), Y);
        }

        public Point2D MirrorY(dynamic y)
        {
            return new Point2D(X, y - (Y-y));
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is Point2D && Equals((Point2D)obj);
        }
    }

    public class Point2DEqualityComparer : IEqualityComparer<Point2D>
    {
        public bool Equals(Point2D x, Point2D y)
        {
            return x.Equals(y);
        }

        public int GetHashCode([DisallowNull] Point2D obj)
        {
            return (obj.X, obj.Y).GetHashCode();
        }
    }
}
