using System;
using System.Globalization;

namespace Devscord.DiscordFramework.Framework.Commands
{
    public class UniversalNumberParser
    {
        public object Parse(string value, Type type)
        {
            return Convert.ChangeType(value.Replace(',', '.'), Nullable.GetUnderlyingType(type) ?? type, CultureInfo.InvariantCulture);
        }
    }
}
