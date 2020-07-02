using System;

namespace Devscord.DiscordFramework.Framework.Architecture.Controllers
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AdminCommand : Attribute
    {

        public AdminCommand()
        {
        }
    }
}
