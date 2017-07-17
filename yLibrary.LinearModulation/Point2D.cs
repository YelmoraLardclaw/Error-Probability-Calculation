using System;

namespace yLibrary.Voronoi
{
    public struct Point : IPoint
    {
        public Point(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public double X { get; set; }
        public double Y { get; set; }

        public bool IsNull => double.IsNaN(X) || double.IsNaN(Y);

        public double Distance(IPoint other) => Distance(this, other);

        public static double Distance(IPoint first, IPoint second) => Math.Sqrt(Math.Pow(first.X - second.X, 2d) + Math.Pow(first.Y - second.Y, 2d));

        public static Point operator +(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);

        public static Point operator -(Point a, Point b) => new Point(a.X - b.X, a.Y - b.Y);

        public static bool operator ==(Point a, Point b) => a.Equals(b);

        public static bool operator !=(Point a, Point b) => !a.Equals(b);

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Point))
                return ((Point)obj).X == X && ((Point)obj).Y == Y;
            else
                return false;
        }

        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();

        public override string ToString() => string.Format("({0};{1})", X, Y);

        public static readonly Point NullPoint = new Point(double.NaN, double.NaN);
    }
}
