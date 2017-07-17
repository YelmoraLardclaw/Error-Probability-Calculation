using System;
using System.Collections.Generic;
using System.Linq;
using yLibrary.Voronoi.Events;

namespace yLibrary.Voronoi
{
    public sealed class Fortune
    {

        private VoronoiDiagram diagram = new VoronoiDiagram();
        /// <summary>
        /// A Voronoi diagram class that includes all the required information to render a finished Voronoi diagram.
        /// </summary>
        public VoronoiDiagram Diagram
        {
            get
            {
                if (IsCalculated)
                    return diagram;
                else
                    return null;
            }
        }

        public bool IsCalculated { get; private set; }

        private List<ParabolaArc> beachline = new List<ParabolaArc>();
        private double sweepLine;
#if DEBUG
        private double eps = 0.00001d;
#endif
        private int iteration = 0;

        private List<HalfEdge> edges = new List<HalfEdge>();

        private EventQueue eventQueue;

        #region Contructors
        /// <summary>
        /// Creates Fortune class to calculate the Voronoi diagram.
        /// </summary>
        /// <param name="SitesAsPoints">Sites represented as points on infinite 2D plane.</param>
        public Fortune(Point[] SitesAsPoints)
        {
            if (SitesAsPoints == null)
                throw new ArgumentNullException(nameof(SitesAsPoints));
            diagram.Sites = SitesAsPoints.Select((x) => new Site(x)).ToArray();
        }

        public Fortune(Site[] Sites)
        {
            //There is some really strong need in validating of Site array.
            if (Sites == null)
                throw new ArgumentNullException(nameof(Sites));

            if (Sites.Any(x => x == null))
                throw new ArgumentNullException(nameof(Sites));
            foreach (Site s in Sites)
                if (Sites.Count(x => x.ID == s.ID) > 1)
                    throw new ArgumentException("There are multiple sites of the same ID.");

            diagram.Sites = Sites;
        }
        #endregion

        /// <summary>
        /// Calculates the whole Voronoi diagram.
        /// </summary>
        /// <returns>The resulting Voronoi diagram</returns>
        public VoronoiDiagram Calculate()
        {
            CalculateDiagram();

            diagram.Edges = edges.ToArray();
            diagram.IsOptimized = false;
            edges = null;
            IsCalculated = true;
            return Diagram;
        }

        /// <summary>
        /// Calculates the whole Voronoi diagram.
        /// </summary>
        /// <returns>The resulting Voronoi diagram</returns>
        public VoronoiDiagram Calculate(FortuneOptions Options)
        {
            CalculateDiagram();

            diagram.Edges = edges.ToArray();
            diagram.IsOptimized = false;
            edges = null;
            IsCalculated = true;

            if (Options.HasFlag(FortuneOptions.Optimize))
                VoronoiOptimizer.Optimize(ref diagram);

            return Diagram;
        }

        /// <summary>
        /// Contains the main logic of Fortune's algorithm.
        /// </summary>
        void CalculateDiagram()
        {
            eventQueue = new EventQueue(diagram.Sites);

            while (!eventQueue.IsEmpty())
            {
                if (eventQueue.Peek() is ArcAppearEvent)
                    ProcessArcAppear(eventQueue.Dequeue() as ArcAppearEvent);
                else if (eventQueue.Peek() is ArcRemoveEvent)
                    ProcessArcRemove(eventQueue.Dequeue() as ArcRemoveEvent);
                iteration++;
            }
        }

        /// <summary>
        /// Processes an event of new arc appearing.
        /// </summary>
        /// <param name="arcAddEvent"></param>
        void ProcessArcAppear(ArcAppearEvent arcAddEvent) //DONE
        {
            sweepLine = arcAddEvent.Y;
            ParabolaArc newArc = new ParabolaArc(arcAddEvent.SiteToAppear, GetIteration, GetSweepLine);
            if (beachline.Count == 0)
            {
                beachline.Add(newArc);
                return;
            }

            int currentElementIndex = beachline.BinarySearch(newArc, new BeachlineInsertionComparer(GetLeftNeighbour, GetRightNeighbour));
            ParabolaArc currentArc = beachline[currentElementIndex];
            RemoveParabolaRemoveEvent(currentArc);
            beachline.Insert(currentElementIndex + 1, newArc); 
            if (newArc.Y != currentArc.Y)
                beachline.Insert(currentElementIndex + 2, new ParabolaArc(currentArc.Site, GetIteration, GetSweepLine));

            Point[] newEdgesStartPoint = ParabolaArc.FindCrosspoints(newArc, currentArc);
            edges.Add(new HalfEdge(newEdgesStartPoint[0], currentArc.Site, newArc.Site));
            edges.Add(new HalfEdge(newEdgesStartPoint[0], newArc.Site, currentArc.Site));

            CheckForRemoveEvent(GetLeftNeighbour(newArc));
            CheckForRemoveEvent(GetRightNeighbour(newArc));
        }

        /// <summary>
        /// Processes an event of arc disappearing.
        /// </summary>
        /// <param name="arcRemoveEvent"></param>
        void ProcessArcRemove(ArcRemoveEvent arcRemoveEvent) 
        {
            sweepLine = arcRemoveEvent.Y;
            ParabolaArc leftArc = GetLeftNeighbour(arcRemoveEvent.ArcToRemove),
                       rightArc = GetRightNeighbour(arcRemoveEvent.ArcToRemove);
            RemoveParabolaRemoveEvent(leftArc);
            RemoveParabolaRemoveEvent(rightArc);

            /* It's never enough checks. Actually checking if the *
             * distances are the same. Probably is going to stay  *
             * only in debug version.                             */
#if DEBUG
            double d1 = arcRemoveEvent.VoronoiVertex.Distance(leftArc),
                d2 = arcRemoveEvent.VoronoiVertex.Distance(rightArc),
                d3 = arcRemoveEvent.VoronoiVertex.Distance(arcRemoveEvent.ArcToRemove);
            if (Math.Abs(d1 - d2) > eps || Math.Abs(d2 - d3) > eps)
                throw new ArgumentException(string.Format("Arc remove event does not happen properly. Distances:\nLeft {0}\nRight {1}\nRemove {2}", d1, d2, d3));
#endif

            #region Managing edge list
            HalfEdge leftEdge = edges.Find(x => x.LeftSite.Equals(leftArc.Site) && x.RightSite.Equals(arcRemoveEvent.ArcToRemove.Site)),
                    rightEdge = edges.Find(x => x.LeftSite.Equals(arcRemoveEvent.ArcToRemove.Site) && x.RightSite.Equals(rightArc.Site));
            leftEdge.CloseEdge(arcRemoveEvent.VoronoiVertex);
            rightEdge.CloseEdge(arcRemoveEvent.VoronoiVertex);
            /* Removing phatom edges - the ones that are of *
             * zero length.                                 */
            if (Point.Distance(leftEdge.A, leftEdge.B) == 0)
                edges.Remove(leftEdge);
            if (Point.Distance(rightEdge.A, rightEdge.B) == 0)
                edges.Remove(rightEdge);

            edges.Add(new HalfEdge(arcRemoveEvent.VoronoiVertex, leftArc.Site, rightArc.Site));
            #endregion

            beachline.Remove(arcRemoveEvent.ArcToRemove);

            CheckForRemoveEvent(leftArc);
            CheckForRemoveEvent(rightArc);
        }

        void CheckForRemoveEvent(ParabolaArc arc) 
        {
            if (arc == null)
                return;

            ParabolaArc leftArc = GetLeftNeighbour(arc),
                       rightArc = GetRightNeighbour(arc);
            if (leftArc == null || rightArc == null || leftArc.Site.Equals(rightArc.Site))
                return;

            HalfEdge leftEdge = edges.Find(x => x.LeftSite.Equals(leftArc.Site) && x.RightSite.Equals(arc.Site)),
                    rightEdge = edges.Find(x => x.LeftSite.Equals(arc.Site) && x.RightSite.Equals(rightArc.Site));


            Point voronoiVertex = HalfEdge.Crosspoint(leftEdge, rightEdge);
            if (voronoiVertex.IsNull)
                return;

            /* This check serves two purposes. One - to check if the   *
             * event happens under sweep line and we should not bother *
             * about it. Two - getting right of a bit of the rounding  *
             * error that inevitably happens while solving equations   *
             * with a computer.                                        */
            double vertexCircleRadius = Point.Distance(voronoiVertex, arc);
            if (voronoiVertex.Y + vertexCircleRadius < sweepLine)
                return;
            
            voronoiVertex.Y += vertexCircleRadius;
            ArcRemoveEvent removeEvent = new ArcRemoveEvent(arc, voronoiVertex, vertexCircleRadius);
            eventQueue.Add(removeEvent);
        }

        public void Clear()
        {
            iteration = 0;
            sweepLine = 0;
            IsCalculated = false;
            diagram.Clear();
        }
        
        public double GetSweepLine() => sweepLine;
        public int GetIteration() => iteration;
        public ParabolaArc GetLeftNeighbour(ParabolaArc Element)
        {
            int index = beachline.IndexOf(Element);
            if (index == -1 || index == 0)
                return null;
            else
                return beachline[index - 1];
        }
        public ParabolaArc GetRightNeighbour(ParabolaArc Element)
        {
            int index = beachline.IndexOf(Element);
            if (index == -1 || index == beachline.Count - 1)
                return null;
            else
                return beachline[index + 1];
        }
        void RemoveParabolaRemoveEvent(ParabolaArc Arc)
        {
            eventQueue.RemoveAll(x =>
            {
                if (x is ArcRemoveEvent)
                {
                    ArcRemoveEvent e = x as ArcRemoveEvent;
                    return e.ArcToRemove.Equals(Arc);
                }
                else
                    return false;
            });
        }
    }

    public enum FortuneOptions
    {
        Optimize = 1
    }
}
