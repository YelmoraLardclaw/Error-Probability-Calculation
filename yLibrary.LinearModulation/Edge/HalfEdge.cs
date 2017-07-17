using System;

namespace yLibrary.Voronoi
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public sealed class HalfEdge : Edge, IEquatable<HalfEdge>
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {
        /// <summary>
        /// First direction point - midpoint between SiteA and SiteB
        /// </summary>
        public Point DirectionPointA => new Point((LeftSite.X + RightSite.X) / 2d, (LeftSite.Y + RightSite.Y) / 2d);
        /// <summary>
        /// Second direction point - 90* rotated vector from SiteA to
        /// SiteB Rotation is simply applied as transoformation of
        /// a(SiteA.Position - SiteB.Position) => b(a.y; -a.x) 
        /// </summary>
        public Point DirectionPointB => new Point(DirectionPointA.X + (LeftSite.Y - RightSite.Y) / 10d, DirectionPointA.Y - (LeftSite.X - RightSite.X) / 10d);
        
        private bool infinite = true;
        internal bool infiniteChange { get { return infinite; } set { infinite = value; } }
        /// <summary>
        /// Specifies, if the edge is infinite on the B point.
        /// </summary>
        public bool IsInfinite => infinite;
        /// <summary>
        /// A special normal vector to line, connecting LeftSide and RightSite
        /// </summary>
        public Point Vector => new Point((LeftSite.Y - RightSite.Y) / 10d, (LeftSite.X - RightSite.X) / -10d);

        public HalfEdge(Point StartPoint, Site LeftSite, Site RightSite)
        {
            this.LeftSite = LeftSite;
            this.RightSite = RightSite;
            a = StartPoint;
            b = a + Vector;
        }

        /// <summary>
        /// Closes the edge on the given point.
        /// </summary>
        /// <param name="End">End point of the edge.</param>
        public void CloseEdge(Point End)
        {
            b = End;
            infinite = false;
        }

        public bool HasPoint(Point Point)
        {
            Point direction = new Point(Math.Sign(Vector.X), Math.Sign(Vector.Y));
            if (infinite &&
                ((direction.X > 0 && a.X <= Point.X) ^
                 (direction.X < 0 && a.X >= Point.X) ^
                 (direction.X == 0 && a.X == Point.X)) &&
                ((direction.Y > 0 && a.Y <= Point.Y) ^
                 (direction.Y < 0 && a.Y >= Point.Y) ^
                 (direction.Y == 0 && a.Y == Point.Y)))
                return true;
            else
                return false;
        }

        public static Point Crosspoint(HalfEdge first, HalfEdge second) //Crosspoint Ariston, haha.
        {
            if (first == null || second == null || !first.infinite || !second.infinite)
                return Point.NullPoint;

            double[] x = { first.DirectionPointA.X, first.DirectionPointB.X, second.DirectionPointA.X, second.DirectionPointB.X },
                     y = { first.DirectionPointA.Y, first.DirectionPointB.Y, second.DirectionPointA.Y, second.DirectionPointB.Y };
            double denominator = (x[0] - x[1]) * (y[2] - y[3]) - (y[0] - y[1]) * (x[2] - x[3]);
            /* If denominator equals zero, we have either parallel, *
             * or coincident lines.                                 */
            if (denominator == 0)
                return new Point(double.NaN, double.NaN);
            Point crosspoint = new Point(((x[0] * y[1] - y[0] * x[1]) * (x[2] - x[3]) - (x[0] - x[1]) * (x[2] * y[3] - y[2] * x[3])) / denominator,
                                             ((x[0] * y[1] - y[0] * x[1]) * (y[2] - y[3]) - (y[0] - y[1]) * (x[2] * y[3] - y[2] * x[3])) / denominator);

            //TODO: check if it's even laying within the borders
            if (first.HasPoint(crosspoint) && second.HasPoint(crosspoint))
                return crosspoint;
            else
                return Point.NullPoint;
        }

        public override bool Equals(object obj)
        {
            if (obj is HalfEdge)
                return Equals(obj as HalfEdge);
            else
                return false;
        }

        public bool Equals(HalfEdge other) => other.LeftSite.ID == LeftSite.ID && other.RightSite.ID == RightSite.ID;
        
        public override string ToString() => string.Format("ID {0}:{1} A({2};{3}) B({4};{5}) {6}", LeftSite?.ID, RightSite?.ID,
                                  a.X.ToString("G"), a.Y.ToString("G"), b.X.ToString("G"), b.Y.ToString("G"), infinite ? double.PositiveInfinity.ToString() : "");

        public FullEdge ToFullEdge() => new FullEdge(a, b, LeftSite, RightSite, false, infinite);
    }
}
