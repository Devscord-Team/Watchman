using System;

namespace Devscord.DiscordFramework.Architecture.Controllers
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ReadAlways : Attribute
    {
        public ReadAlways()
        {
        }
    }
}
