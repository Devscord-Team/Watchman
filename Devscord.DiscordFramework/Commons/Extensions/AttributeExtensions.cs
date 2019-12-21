using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;

namespace Devscord.DiscordFramework.Commons.Extensions
{
    public static class AttributeExtensions
    {
        public static IEnumerable<CustomAttributeData> GetAttributes<T>(this IEnumerable<CustomAttributeData> customAttributes) where T : Attribute
        {
            return customAttributes.Where(x => x.AttributeType.FullName == typeof(T).FullName);
        }
    }
}
