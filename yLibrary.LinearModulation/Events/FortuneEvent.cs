using System;

namespace yLibrary.Voronoi.Events
{
    public abstract class FortuneEvent : IComparable, IPoint
    {
        public Point Position { get; protected set; }

        public double X => Position.X;

        public double Y => Position.Y;

        public int CompareTo(object obj)
        {
            if (obj is FortuneEvent)
            {
                FortuneEvent anotherEvent = (FortuneEvent)obj;
                if (Position.Y > anotherEvent.Position.Y)
                    return 1;
                else if (Position.Y < anotherEvent.Position.Y)
                    return -1;
                else if (Position.X > anotherEvent.Position.X)
                    return 1;
                else if (Position.X < anotherEvent.Position.X)
                    return -1;
                else if (GetType() != anotherEvent.GetType())
                {
                    if (anotherEvent is ArcAppearEvent)
                        return 1;
                    else
                        return -1;
                }
                else return 0;
            }
            else
                return -2;
        }
        public override bool Equals(object obj)
        {
            if (obj is FortuneEvent && GetType() == obj.GetType())
            {
                if (this is ArcRemoveEvent)
                    return (this as ArcRemoveEvent).Equals(obj as ArcRemoveEvent);
                else if (this is ArcAppearEvent)
                    return (this as ArcAppearEvent).Equals(obj as ArcAppearEvent);
                return false;
            }
            else
                return false;
        }

        public override int GetHashCode() => Position.GetHashCode();

        public override string ToString() => string.Format("{0} X:{1} Y:{2}", GetType().Name, X, Y);
    }
}
