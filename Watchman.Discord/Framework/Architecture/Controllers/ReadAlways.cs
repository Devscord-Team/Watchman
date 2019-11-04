using System;
using System.Collections.Generic;
using System.Text;

namespace Watchman.Discord.Framework.Architecture.Controllers
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ReadAlways : Attribute
    {
        public ReadAlways()
        {
        }
    }
}
