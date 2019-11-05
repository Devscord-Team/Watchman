using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Watchman.Common.Extensions
{
    public static class MethodExtensions
    {
        public static bool HasAttribute<T>(this MethodInfo method) where T : Attribute
        {
            return method.CustomAttributes.Any(a => a.AttributeType.FullName == typeof(T).FullName);
        }

        public static bool HasParameter<T>(this MethodInfo method)
        {
            return method.GetParameters().Any(p => p.ParameterType.FullName == typeof(T).FullName);
        }
    }
}
