using System;

namespace yLibrary.Voronoi
{
    /// <summary>
    /// Parabola class for the Fortune's method Beachline. Is defined with it's Focus Point and Directrix
    /// </summary>
    public class ParabolaArc : BeachlineElement, IPoint, IEquatable<ParabolaArc>
    {
        private bool IsDegenerated = true;
        private double degenX;
        /// <summary>
        /// A site of Voronoi diagram to which parabola arc belongs.
        /// </summary>
        public Site Site { get; }

        /// <summary>
        /// Focus point of the parabola.
        /// </summary>
        public Point Focus => Site.Position;
        /// <summary>
        /// 
        /// </summary>
        public double X => Site.Position.X;
        /// <summary>
        /// 
        /// </summary>
        public double Y => Site.Position.Y;
        
        /// <summary>
        /// Delegate to get the directrix at the given moment.
        /// </summary>
        protected Func<double> GetDirectrix;

        #region Calculated parameters
        private double a = 0d;
        /// <summary>
        /// A coefficient of quadratic equation f(x) = A*x^2 + B*x + C
        /// </summary>
        public double A
        {
            get
            {
                if (lastCalcIteration != GetCurrentIteration())
                    Recalculate();
                return a;
            }
        }
        private double b = 0d;
        /// <summary>
        /// B coefficient of quadratic equation f(x) = A*x^2 + B*x + C
        /// </summary>
        public double B
        {
            get
            {
                if (lastCalcIteration != GetCurrentIteration())
                    Recalculate();
                return b;
            }
        }
        private double c = 0d;
        /// <summary>
        /// C coefficient of quadratic equation f(x) = A*x^2 + B*x + C
        /// </summary>
        public double C
        {
            get
            {
                if (lastCalcIteration != GetCurrentIteration())
                    Recalculate();
                return c;
            }
        }
        #endregion

        public ParabolaArc(Site Site, Func<int> GetCurrentIteration, Func<double> GetDirectrix) : base()
        {
            this.Site = Site;
            this.GetCurrentIteration = GetCurrentIteration;
            this.GetDirectrix = GetDirectrix;
            Recalculate();
        }

        #region Math
        public double Fx(double X) => IsDegenerated ? (X == degenX ? double.PositiveInfinity : double.NaN) : A * Math.Pow(X, 2d) + B * X + C;
        
        /// <summary>
        /// Finds the intersection points of two parabolas.
        /// </summary>
        /// <param name="First">First parabola.</param>
        /// <param name="Second">Second parabola.</param>
        /// <returns></returns>
        public static Point[] FindCrosspoints(ParabolaArc First, ParabolaArc Second)
        {
            if (First == null || Second == null)
                return new Point[0];

            Point[] points;
            double A = First.A - Second.A,
                   B = First.B - Second.B,
                   C = First.C - Second.C;
            
            if(First.IsDegenerated || Second.IsDegenerated)
            {
                if (First.IsDegenerated && Second.IsDegenerated)
                    return new Point[] { new Point((First.Focus.X + Second.Focus.X) / 2, First.Focus.Y) };
                else
                {
                    points = new Point[1];
                    points[0] = First.IsDegenerated ? new Point(First.degenX, Second.Fx(First.degenX)) :
                                                      new Point(Second.degenX, First.Fx(Second.degenX));
                    return points;
                }
            }

            if (A != 0)
            {
                double discriminantRoot = Math.Sqrt(Math.Pow(B, 2d) - 4d * A * C);

                if (discriminantRoot > 0d)
                {
                    //Two roots
                    points = new Point[2];
                    points[0].X = (-B + discriminantRoot) / 2d / A;
                    points[0].Y = First.Fx(points[0].X);
                    points[1].X = (-B - discriminantRoot) / 2d / A;
                    points[1].Y = First.Fx(points[1].X);
                }
                else if (discriminantRoot == 0d)
                {
                    //One root
                    points = new Point[1];
                    points[0].X = -B / 2d / A;
                    points[0].Y = First.Fx(points[0].X);
                }
                else
                    points = new Point[0];
            }
            else if (B != 0)
                points = new Point[] { new Point(-C / B, First.Fx(-C / B)) };
            else
                throw new SameParabolaException(First, Second);

            return points;
        }

        /// <summary>
        /// Recalculates the coefficients of parabola equation of form 'f(x) = A*x^2 + B*x +C'
        /// </summary>
        public void Recalculate()
        {
            //Checking if we can get the directrix of the parabola or not. If we're unable to, we can't proceed.
            if (GetDirectrix == null)
                throw new ArgumentException("GetDirectrix delegate is null, unable to calculate.");

            //Some coefficients to be further used not once
            //
            //    A tiny bit of optimization
            //         ||
            //         \/
            double k = GetDirectrix(),
                bMinusK = Site.Position.Y - k;

            //Parameters that has to be changed at every iteration.
            if (bMinusK == 0)
                degenX = Site.Position.X;
            else
            {
                IsDegenerated = false;
                a = 0.5d / bMinusK;
                b = -Site.Position.X / bMinusK;
                c = 0.5d * (Math.Pow(Site.Position.X, 2d) / bMinusK + Site.Position.Y + k);
            }

            lastCalcIteration = GetCurrentIteration();
        }
        #endregion

        public override bool Equals(object obj)
        {
            if (obj is ParabolaArc)
                return Equals(obj as ParabolaArc);
            else return false;
        }

        public bool Equals(ParabolaArc other) => ID == other.ID;

        public override int GetHashCode() => Site.GetHashCode() ^ 1176; //why this number? 'parabola arc' in ASCII code, all codes summed up.

        public override string ToString()  => base.ToString() + string.Format(" SID {0} f(x) = {1} * x^2 + {2} * x + {3}", Site.ID, A, B, C);
        
        [Serializable]
        public class SameParabolaException : Exception
        {
            private int ID1, ID2;
            public new string Message { get; }
            public SameParabolaException(ParabolaArc First, ParabolaArc Second) : base()
            {
                ID1 = First.ID;
                ID2 = Second.ID;
                Message = base.Message + string.Format("Impossible to find crosspoints of parabolas ID:{0} and ID:{1} as they are have the same graphics.", ID1, ID2);
            }
        }
    }
}
