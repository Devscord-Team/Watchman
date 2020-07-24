using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devscord.DiscordFramework.Commons.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object obj, bool indented = true)
        {
            return JsonConvert.SerializeObject(obj, (Formatting)(indented ? 1 : 0));
        }
    }
}
