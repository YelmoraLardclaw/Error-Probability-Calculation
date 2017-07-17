namespace yLibrary.Voronoi.Events
{
    public sealed class ArcAppearEvent : FortuneEvent
    {
        public Site SiteToAppear { get; }

        public ArcAppearEvent(Site Site)
        {
            SiteToAppear = Site;
            Position = SiteToAppear.Position;
        }

        public bool Equals(ArcAppearEvent other) => SiteToAppear.ID == other.SiteToAppear.ID;
    }
}
