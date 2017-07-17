using System;

namespace yLibrary.Voronoi
{
    public class Site : IPoint, IEquatable<Site>
    {
        private static int IDCounter = 0;
        public int ID { get; } = IDCounter++;
        
        public Point Position { get; }
        public double X => Position.X;
        public double Y => Position.Y;

        public Site(double X, double Y)
        {
            Position = new Point(X, Y);
        }
    
        public Site(Point Position)
        {
            this.Position = Position;
        }

        public override bool Equals(object obj)
        {
            if (obj is Site)
                return Equals(obj as Site);
            else
                return false;
        }

        public bool Equals(Site other) => other.X == X && other.Y == Y;

        public override int GetHashCode() => Position.GetHashCode() ^ ID.GetHashCode();

        public override string ToString() => string.Format("ID:{0} ({1};{2})", ID, X, Y);
    }
}
