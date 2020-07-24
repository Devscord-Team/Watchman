using System;

namespace Devscord.EventStore
{
    internal static class Mapper
    {
        internal static object Map(object input, Type outputType)
        {
            var mappedEvent = Activator.CreateInstance(outputType);
            Copy(input, mappedEvent);
            return mappedEvent;
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

