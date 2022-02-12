using System;

namespace Devscord.DiscordFramework.Architecture.Controllers
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AdminCommand : Attribute
    {

        public AdminCommand()
        {
        }
    }
}
