using System;

namespace Devscord.DiscordFramework.Framework.Architecture.Controllers
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ReadAlways : Attribute
    {
        public ReadAlways()
        {
        }
    }
}
