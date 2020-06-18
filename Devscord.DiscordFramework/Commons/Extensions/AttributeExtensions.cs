using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Devscord.DiscordFramework.Commons.Extensions
{
    public static class AttributeExtensions
    {
        public static IEnumerable<CustomAttributeData> FilterAttributes<T>(this IEnumerable<CustomAttributeData> customAttributes) where T : Attribute => customAttributes.Where(x => x.AttributeType.FullName == typeof(T).FullName);
    }
}
