using System;
using System.Collections.Generic;

namespace Devscord.EventStore
{
    public static class EventStore //todo - save events
    {
        private static List<KeyValuePair<string, object>> _events = new List<KeyValuePair<string, object>>();
        private static List<KeyValuePair<string, Action<object>>> _eventHandlers = new List<KeyValuePair<string, Action<object>>>();

        public static void Publish<T>(T @event) where T : IEvent 
        {

        }

        public static void Subscribe<T>(Action<T> action) where T : IEvent
        {

        }
    }

    public interface IEvent
    {
    }
}
