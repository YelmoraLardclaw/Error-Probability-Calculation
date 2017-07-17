using System;
using System.Collections.Generic;

namespace yLibrary.Voronoi
{
    public class BeachlineInsertionComparer : IComparer<ParabolaArc>
    {
        public const int TO_RIGHT = -1,
                          TO_LEFT = 1,
                            STOP = 0;

        public Func<ParabolaArc, ParabolaArc> GetLeftNeighbour, GetRightNeighbour;

        public BeachlineInsertionComparer(Func<ParabolaArc, ParabolaArc> GetLeftNeighbour, Func<ParabolaArc, ParabolaArc> GetRightNeighbour)
        {
            this.GetLeftNeighbour = GetLeftNeighbour;
            this.GetRightNeighbour = GetRightNeighbour;
        }

        /// <summary>
        /// Used to search the place in Beachline where to put in the new parabola.
        /// </summary>
        /// <param name="First">Parabola from the array.</param>
        /// <param name="Second">New parabola.</param>
        /// <returns></returns>
        public int Compare(ParabolaArc currentArc, ParabolaArc newArc)
        {
            ParabolaArc left = GetLeftNeighbour(currentArc),
                       right = GetRightNeighbour(currentArc);

            if (currentArc.Y == newArc.Y && right == null)
                return STOP;

            if (left != null)
            {
                Point[] leftBreakpoint = ParabolaArc.FindCrosspoints(left, currentArc);
                if (leftBreakpoint.Length == 2)
                    if (leftBreakpoint[0].X > left.X && leftBreakpoint[0].X < currentArc.X)
                        leftBreakpoint = new Point[] { leftBreakpoint[0] };
                    else
                        leftBreakpoint = new Point[] { leftBreakpoint[1] };
                if (newArc.X < leftBreakpoint[0].X)
                    return TO_LEFT;
            }

            if (right != null)
            {
                Point[] rightBreakpoint = ParabolaArc.FindCrosspoints(currentArc, right);
                if (rightBreakpoint.Length == 2)
                    if (rightBreakpoint[0].X < right.X && rightBreakpoint[0].X > currentArc.X)
                        rightBreakpoint = new Point[] { rightBreakpoint[0] };
                    else
                        rightBreakpoint = new Point[] { rightBreakpoint[1] };
                if (newArc.X > rightBreakpoint[0].X)
                    return TO_RIGHT;
            }

            return STOP;
        }
    }
}
