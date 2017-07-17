using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace yLibrary.Voronoi.Events
{
    public class EventQueue : IEnumerable
    {
        private List<FortuneEvent> listOfEvents = new List<FortuneEvent>();
        /// <summary>
        /// Internal member for testing purposes.
        /// </summary>
        internal List<FortuneEvent> Events
        {
            get { return listOfEvents; }
            set { listOfEvents = value; }
        }

        /// <summary>
        /// Indexer to get a certain event of the queue.
        /// </summary>
        /// <param name="index">Index of the event, range [0,n].</param>
        /// <returns>FortuneEvent object from the queue at index.</returns>
        public FortuneEvent this[int index] => listOfEvents[index];

        public EventQueue() { }

        public EventQueue(Site[] Sites)
        {
            foreach (Site s in Sites)
                Add(new ArcAppearEvent(s));
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(EventQueue))
                return listOfEvents.SequenceEqual(((EventQueue)obj).listOfEvents);
            else
                return false;
        }

        public override int GetHashCode() => listOfEvents.GetHashCode() ^ 123464;

        #region Element interaction
        /// <summary>
        /// Adds the item, using the provided comparator.
        /// </summary>
        /// <param name="Item">An item to add to the queue.</param>
        public void Add(FortuneEvent Item)
        {
            if (listOfEvents.Count == 0)
                listOfEvents.Add(Item);
            else if (listOfEvents[0].CompareTo(Item) == 1)
                listOfEvents.Insert(0, Item);
            else if (listOfEvents.Last().CompareTo(Item) == -1)
                listOfEvents.Add(Item);
            else
            {
                for (int i = 0; i < Events.Count - 1; i++)
                    if (Events[i].CompareTo(Item) != Events[i + 1].CompareTo(Item))
                    {
                        Events.Insert(i + 1, Item);
                        return;
                    }
                listOfEvents.Add(Item);
            }
        }

        /// <summary>
        /// Adds the item to the end of the EventQueue.
        /// </summary>
        /// <param name="Item">Item of type T to add.</param>
        public void Enqueue(FortuneEvent Item) => Events.Add(Item);

        /// <summary>
        /// Takes the first item of the EventQueue.
        /// </summary>
        /// <returns>Item from the start of the EventQueue.</returns>
        public FortuneEvent Dequeue()
        {
            FortuneEvent Item = Events.First();
            Events.Remove(Item);
            return Item;
        }

        /// <summary>
        /// Returns the first element on the EventQueue.
        /// </summary>
        /// <returns>First element of the EventQueue.</returns>
        public FortuneEvent Peek() => Events.First();

        /// <summary>
        /// Removes all the occurences of FortuneEvent objects that satifies the Predicate<EventQueue> rule.
        /// </summary>
        /// <param name="predicate">Predicate rule to choose objects.</param>
        /// <returns>Number of objects removed.</returns>
        public int RemoveAll(Predicate<FortuneEvent> predicate) => listOfEvents.RemoveAll(predicate);
        #endregion

        public bool IsEmpty() => listOfEvents.Count == 0;

        public IEnumerator GetEnumerator() => listOfEvents.GetEnumerator();
    }
}
