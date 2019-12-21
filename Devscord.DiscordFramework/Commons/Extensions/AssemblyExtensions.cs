using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Devscord.DiscordFramework.Commons.Extensions
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Type> GetTypesByInterface<T>(this Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(x => x.GetInterfaces().Any(type => type.FullName == typeof(T).FullName));
        }
    }
}
