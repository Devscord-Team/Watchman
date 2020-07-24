using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devscord.EventStore
{
    internal class EventBus<T> : IEventBus where T : Event
    {
        private readonly List<Action<T>> _eventHandlers = new List<Action<T>>();

        internal void AddEventHandler(Action<T> eventHandler)
        {
            this._eventHandlers.Add(eventHandler);
        }

        internal void Run(object @event)
        {
            var casted = (T) @event;
            Parallel.ForEach(this._eventHandlers, x => x.Invoke(casted));
        }
    }
}

