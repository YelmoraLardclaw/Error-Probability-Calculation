using System;
using System.Collections.Generic;
using System.Linq;

namespace yLibrary.Voronoi
{
    public class VoronoiOptimizer
    {
        public static void Optimize(ref VoronoiDiagram Diagram)
        {
            if (!Diagram.IsOptimized)
            {
                List<FullEdge> finalEdgeList = new List<FullEdge>();
                List<HalfEdge> edgesToOptimize = (Diagram.Edges as HalfEdge[]).ToList();
                while(edgesToOptimize.Count != 0)
                {
                    HalfEdge firstEdge = edgesToOptimize[0];
                    edgesToOptimize.RemoveAt(0);
                    HalfEdge secondEdge = edgesToOptimize.Find(x => x.LeftSite == firstEdge.RightSite &&
                                                                    x.RightSite == firstEdge.LeftSite);
                    if (secondEdge == null)
                        finalEdgeList.Add(firstEdge.ToFullEdge());
                    else
                    {
                        edgesToOptimize.Remove(secondEdge);
                        finalEdgeList.Add(Merge(firstEdge, secondEdge));
                    }
                }
                Diagram.Edges = finalEdgeList.ToArray();
            }
        }

        public static FullEdge Merge(HalfEdge HE1, HalfEdge HE2)
        {
            if (HE1.LeftSite == HE2.RightSite && HE1.RightSite == HE2.LeftSite)
                return new FullEdge(HE1.B, HE2.B, HE1.LeftSite, HE1.RightSite, HE1.IsInfinite, HE2.IsInfinite);
            else
                throw new ArgumentException("Unable to merge edges, because they do not belong to the same pair of sites.");
        }
    }
}
