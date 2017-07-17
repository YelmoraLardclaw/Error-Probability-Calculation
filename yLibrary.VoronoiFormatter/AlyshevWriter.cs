using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static yLibrary.Voronoi.VoronoiDiagram;

namespace yLibrary.Voronoi.Formatter
{
    public class AlyshevWriter
    {
        private string path, doubleFormat;
        StreamWriter writer;

        public string DoubleFormat => doubleFormat;

        public AlyshevWriter(string path, string doubleFormat)
        {
            this.path = path;
            this.doubleFormat = doubleFormat;
        }

        public void Write(VoronoiDiagram diagram)
        {
            writer = new StreamWriter(path, false);
            if (Path.GetDirectoryName(path) != "" && !Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));

            writer.WriteLine(diagram.Sites.Length);
            foreach (Site s in diagram.Sites)
                writer.WriteLine(string.Format("{0} {1}", s.X.ToString(doubleFormat), s.Y.ToString(doubleFormat)));

            try
            {
                if (diagram.IsOptimized)
                    WriteFullEdges(diagram, (diagram.Edges as FullEdge[]).ToList());
                else
                    WriteHalfEdges(diagram, (diagram.Edges as HalfEdge[]).ToList());
            }
            finally
            {
                writer.Close();
                writer.Dispose();
            }
        }

        void WriteHalfEdges(VoronoiDiagram diagram, List<HalfEdge> edges)
        {
            if (edges == null)
                throw new VDiagramStructureException("HalfEdge array has not beed casted. Check the Voronoi diagram.");

            for(int i = 0; i < diagram.Sites.Length; i++)
            {
                bool isOpen;
                List<HalfEdge> currentEdges = edges.FindAll(x => x.LeftSite.ID == diagram.Sites[i].ID || x.RightSite.ID == diagram.Sites[i].ID);
                List<Point> currentSiteVerticles;

                if(currentEdges.Any(x => x.IsInfinite))
                {
                    currentSiteVerticles = GetSortedVerticlesList(currentEdges.First(x => x.IsInfinite), currentEdges);
                    isOpen = true;
                }
                else
                {
                    currentSiteVerticles = GetSortedVerticlesList(currentEdges[0], currentEdges);
                    currentSiteVerticles.RemoveAt(currentSiteVerticles.Count - 1);
                    isOpen = false;
                }

                writer.WriteLine(isOpen ? -1 * currentSiteVerticles.Count : currentSiteVerticles.Count);
                foreach (Point p in currentSiteVerticles)
                    writer.WriteLine(p.X.ToString(doubleFormat) + " " + p.Y.ToString(doubleFormat));
            }
        }

        void WriteFullEdges(VoronoiDiagram diagram, List<FullEdge> edges) //TODO
        {
            if (edges == null)
                throw new VDiagramStructureException("FullEdge array has not beed casted. Check the Voronoi diagram.");
        }

        List<Point> GetSortedVerticlesList(HalfEdge startEdge, List<HalfEdge> currentEdges)
        {
            List<Point> sortedVerticleList = new List<Point>();
            Point nextPoint = startEdge.A;
            currentEdges.Remove(startEdge);
            sortedVerticleList.Add(startEdge.B);

            while (currentEdges.Count > 0)
            {
                startEdge = currentEdges.Find(x => x.A.Equals(nextPoint) || x.B.Equals(nextPoint));
                sortedVerticleList.Add(nextPoint);
                nextPoint = startEdge.A == nextPoint ? startEdge.B : startEdge.A;
                currentEdges.Remove(startEdge);
            }

            sortedVerticleList.Add(nextPoint);

            return sortedVerticleList;
        }
    }
}
