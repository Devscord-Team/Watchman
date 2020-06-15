using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Devscord.DiscordFramework.Commons.Extensions
{
    public static class MethodExtensions
    {
        public static bool HasAttribute<T>(this MethodInfo method) where T : Attribute
        {
            return method.CustomAttributes.Any(a => a.AttributeType.FullName == typeof(T).FullName);
        }

        public static bool HasParameter<T>(this MethodInfo method)
        {
            return method.GetParameters().Any(p => p.ParameterType.FullName == typeof(T).FullName || typeof(T).IsAssignableFrom(p.ParameterType));
        }

        public static bool HasParameter<T>(this ConstructorInfo method)
        {
            return method.GetParameters().Any(p => p.ParameterType.FullName == typeof(T).FullName);
        }

        public static bool HasConstructorParameter<T>(this Type type)
        {
            var constructors = type.GetConstructors();
            if (!constructors.Any())
            {
                return false;
            }
            return constructors.Any(c => c.HasParameter<T>());
        }

        public static IEnumerable<MethodInfo> FilterMethodsByAttribute<T>(this IEnumerable<MethodInfo> methods)
        {
            return methods.Where(x => x.CustomAttributes.Any(a => a.AttributeType.FullName == typeof(T).FullName));
        }

        public static IEnumerable<T> GetAttributeInstances<T>(this MethodInfo method) where T : Attribute
        {
            var commandArguments = method.GetCustomAttributesData()
                .Where(x => x.AttributeType.FullName == typeof(T).FullName)
                .SelectMany(x => x.ConstructorArguments, (x, arg) => arg.Value).ToArray();
            return commandArguments.Select(x => (T)Activator.CreateInstance(typeof(T), x));
        }
    }
}
