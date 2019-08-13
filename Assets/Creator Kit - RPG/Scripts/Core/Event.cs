using System.Collections.Generic;
using RPGM.Gameplay;

namespace RPGM.Core
{
    /// <summary>
    /// An event allows execution of some logic to be deferred for a period of time.
    /// </summary>
    /// <typeparam name="Event"></typeparam>
    public abstract class Event : System.IComparable<Event>
    {
        public virtual void Execute() { }

        protected GameModel model = Schedule.GetModel<GameModel>();

        internal float tick;

        public int CompareTo(Event other)
        {
            return tick.CompareTo(other.tick);
        }

        internal virtual void ExecuteEvent() => Execute();

        internal virtual void Cleanup()
        {

        }
    }

    /// <summary>
    /// Add functionality to the Event class to allow the observer / subscriber pattern.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Event<T> : Event where T : Event<T>
    {
        public static System.Action<T> OnExecute;

        internal override void ExecuteEvent()
        {
            Execute();
            OnExecute?.Invoke((T)this);
        }
    }

}