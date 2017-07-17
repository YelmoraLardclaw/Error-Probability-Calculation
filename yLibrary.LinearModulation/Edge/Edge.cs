namespace yLibrary.Voronoi
{
    public abstract class Edge
    {
        protected Point a, b;

        /// <summary>
        /// 'A' point of the edge A-B.
        /// </summary>
        public Point A => a;

        /// <summary>
        /// 'B' point of the edge A-B.
        /// </summary>
        public Point B => b;

        /// <summary>
        /// Left site of the edge.
        /// </summary>
        public Site LeftSite { get; protected set; }
        /// <summary>
        /// Right site of the edge.
        /// </summary>
        public Site RightSite { get; protected set; }

        public override int GetHashCode() => LeftSite.GetHashCode() ^ RightSite.GetHashCode();
    }
}
