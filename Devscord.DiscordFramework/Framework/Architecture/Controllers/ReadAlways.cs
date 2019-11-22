using System;
using System.Collections.Generic;
using System.Text;

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
