using System;

namespace yLibrary.Voronoi
{
    public abstract class BeachlineElement
    {
        private static int IDCount = 0;
        private int IDnum;
        public int ID => IDnum;

        /// <summary>
        /// Iteration of last coodinate calculation. 
        /// </summary>
        protected int lastCalcIteration = -1;

        /// <summary>
        /// Delegate to get the number of current iteration.
        /// </summary>
        protected Func<int> GetCurrentIteration;

        public BeachlineElement()
        {
            IDnum = IDCount++;
        }

        public override string ToString() => "ID:" + ID;
    }
}
