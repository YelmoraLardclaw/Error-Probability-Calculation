namespace yLibrary.Voronoi
{
    public class FullEdge : Edge
    {
        public bool IsInfiniteA { get; }
        public bool IsInfiniteB { get; }

        public FullEdge(Point A, Point B, Site SiteA, Site SiteB, bool IsInfiniteA, bool IsInfiniteB)
        {
            LeftSite = SiteA;
            RightSite = SiteB;
            a = A;
            b = B;
            this.IsInfiniteA = IsInfiniteA;
            this.IsInfiniteB = IsInfiniteB;
        }

        public override string ToString() => string.Format("ID {0}:{1} A({2};{3}){4} B({5};{6}){7}", LeftSite?.ID, RightSite?.ID,
                                  a.X, a.Y, IsInfiniteA ? double.PositiveInfinity.ToString() : "",
                                  b.X, b.Y, IsInfiniteB ? double.PositiveInfinity.ToString() : "");
    }
}
