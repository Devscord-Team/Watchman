using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devscord.DiscordFramework.Framework.Architecture.Controllers
{
    [AttributeUsage(AttributeTargets.Method)]
    class AdminCommand : Attribute
    {

        public AdminCommand()
        {
        }
    }
}
