using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Devscord.EventStore
{
    public static class EventStore //in memory event "store" TODO change it to correct event store
    {
        private static readonly List<IEventBus> _eventBuses = new List<IEventBus>();

        public static void Initialize(Assembly assembly)
        {
            var eventTypes = assembly.GetTypes().Where(x => x.IsAssignableFrom(typeof(Event))).ToList();
            eventTypes.ForEach(x => GetOrAddEventBus(x));
        }

        public static Task Publish<T>(T @event) where T : Event
        {
            var eventBus = GetOrAddEventBus(@event.GetType());
            var mapTo = eventBus.GetType().GenericTypeArguments.First();
            var mapped = Mapper.Map(@event, mapTo);
            ((dynamic)eventBus).Run(mapped);
            return Task.CompletedTask;
        }

        public static void Subscribe<T>(Action<T> action) where T : Event
        {
            var type = action.Method.GetParameters().First().ParameterType;
            var eventBus = (dynamic) GetOrAddEventBus(type);
            eventBus.AddEventHandler(action);
        }

        private static IEventBus GetOrAddEventBus(Type type)
        {
            var eventBus = _eventBuses.FirstOrDefault(x => x.GetType().GenericTypeArguments.Any(generic => generic.Name == type.Name));
            if(eventBus == null)
            {
                eventBus = (IEventBus) Activator.CreateInstance(typeof(EventBus<>).MakeGenericType(type));
                _eventBuses.Add(eventBus);
            }
            return eventBus;
        }
    }
}

