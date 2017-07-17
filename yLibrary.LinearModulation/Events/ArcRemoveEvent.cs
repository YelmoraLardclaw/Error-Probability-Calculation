namespace yLibrary.Voronoi.Events
{
    public sealed class ArcRemoveEvent : FortuneEvent
    {
        public double CircleRadius { get; }

        public Point VoronoiVertex => new Point(X, Y - CircleRadius);

        public ParabolaArc ArcToRemove { get; }

        public ArcRemoveEvent(ParabolaArc ArcToRemove, Point Position, double CircleRadius)
        {
            this.CircleRadius = CircleRadius;
            this.ArcToRemove = ArcToRemove;
            base.Position = Position;
        }

        public bool Equals(ArcRemoveEvent other) => ArcToRemove.ID == other.ArcToRemove.ID &&
                                                    VoronoiVertex.Equals(other.VoronoiVertex);
    }
}
