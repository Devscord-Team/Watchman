using AutoMapper;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.EventStore
{
    public static class EventStore
    {
        private static readonly Mapper _mapper = new Mapper(new MapperConfiguration(x => { }));
        private static List<KeyValuePair<string, Action<object>>> _eventHandlers = new List<KeyValuePair<string, Action<object>>>();

        public static void Publish<T>(T @event) where T : IEvent 
        {
            var eventName = typeof(T).Name;
            //todo save event
            RunHandlers(eventName, @event);
        }

        public static void Subscribe<T>(Action<T> action) where T : IEvent
        {
            var eventName = typeof(T).Name;
            _eventHandlers.Add(new KeyValuePair<string, Action<object>>(eventName, (dynamic)action));
        }

        private static void RunHandlers<T>(string eventName, T @event) where T : IEvent
        {
            var handlers = _eventHandlers.Where(x => x.Key == eventName);
            foreach (var handler in handlers)
            {
                try
                {
                    var handlerEventType = handler.Value.Method.GetParameters().First().ParameterType;
                    var mappedEvent = _mapper.Map(@event, typeof(T), handlerEventType);
                    handler.Value.Invoke(mappedEvent);
                }
                catch (Exception ex)
                {
                    Log.Error("Cannot run handler for event {event}", JsonConvert.SerializeObject(@event, Formatting.Indented));
                }
            }
        }
    }
}
