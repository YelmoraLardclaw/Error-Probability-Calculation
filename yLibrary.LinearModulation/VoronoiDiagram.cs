using System;

namespace yLibrary.Voronoi
{
    public class VoronoiDiagram
    {
        public Site[] Sites { get; internal set; }
        public Edge[] Edges { get; internal set; }
        public bool IsOptimized { get; internal set; }

        public VoronoiDiagram() { }

        public VoronoiDiagram(Site[] sites)
        {
            Sites = sites;
        }

        internal void Clear()
        {
            Edges = null;
        }

        [Serializable]
        public class VDiagramStructureException : Exception
        {
            public VDiagramStructureException(string message) : base(message)
            { }
        }
    }
}