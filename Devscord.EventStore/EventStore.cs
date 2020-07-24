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
        private static readonly List<KeyValuePair<string, Action<Event>>> _eventHandlers = new List<KeyValuePair<string, Action<Event>>>();

        public static async Task Publish<T>(T @event) where T : Event
        {
            //todo save event
            await RunHandlers(@event.GetType().Name, @event);
        }

        public static void Subscribe<T>(Action<T> action) where T : Event
        {
            var casted = (Action<Event>) action;
            var eventName = typeof(T).Name;
            _eventHandlers.Add(new KeyValuePair<string, Action<Event>>(eventName, casted));
        }

        private static Task RunHandlers<T>(string eventName, T @event) where T : Event
        {
            var handlers = _eventHandlers.Where(x => x.Key == eventName);
            foreach (var handler in handlers)
            {
                try
                {
                    var eventType = @event.GetType();
                    var handlerEventType = handler.Value.Method.GetParameters()[0].ParameterType;

                    var mappedEvent = (Event) Activator.CreateInstance(handlerEventType);
                    Copy(@event, mappedEvent);
                    handler.Value.Invoke(mappedEvent); 
                }
                catch (Exception ex)
                {
                    Log.Error("Cannot run handler for event {event}", JsonConvert.SerializeObject(@event, Formatting.Indented));
                }
            }
            return Task.CompletedTask;
        }

        private static void Copy(object input, object output) //todo move to commons
        {
            var parentProperties = input.GetType().GetProperties();
            var childProperties = output.GetType().GetProperties();

            foreach (var parentProperty in parentProperties)
            {
                foreach (var childProperty in childProperties)
                {
                    if (parentProperty.Name == childProperty.Name && parentProperty.PropertyType == childProperty.PropertyType)
                    {
                        childProperty.SetValue(output, parentProperty.GetValue(input));
                        break;
                    }
                }
            }
        }
    }
}

