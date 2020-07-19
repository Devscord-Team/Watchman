using AutoMapper;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Devscord.EventStore
{
    public static class EventStore //in memory event "store" TODO change it to correct event store
    {
        private static readonly Mapper _mapper = new Mapper(new MapperConfiguration(x => { }));
        private static readonly List<KeyValuePair<string, dynamic>> _eventHandlers = new List<KeyValuePair<string, dynamic>>();

        public static async Task Publish<T>(string eventName, T @event) where T : Event
        {
            //todo save event
            await RunHandlers(eventName, @event);
        }

        public static void Subscribe<T>(Action<T> action) where T : Event
        {
            var eventName = typeof(T).Name;
            _eventHandlers.Add(new KeyValuePair<string, dynamic>(eventName, action));
        }

        private static Task RunHandlers<T>(string eventName, T @event) where T : Event
        {
            var handlers = _eventHandlers.Where(x => x.Key == eventName);
            Parallel.ForEach(handlers, handler =>
            {
                try
                {
                    var method = (MethodInfo) handler.Value.Method;
                    var parameters = method.GetParameters();
                    var handlerEventType = parameters.FirstOrDefault().ParameterType;
                    var mappedEvent = (Event) _mapper.Map(@event, @event.GetType(), handlerEventType);
                    handler.Value.Invoke(mappedEvent);
                }
                catch (Exception ex)
                {
                    Log.Error("Cannot run handler for event {event}", JsonConvert.SerializeObject(@event, Formatting.Indented));
                }
            });
            return Task.CompletedTask;
        }
    }
}
