using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Watchman.Discord.Framework.Architecture.Controllers
{
    [AttributeUsage(AttributeTargets.Method)]
    class AdminCommand : Attribute
    {
        public bool IsRequestedByAdmin { get; }

        public AdminCommand()
        {
        }

        public AdminCommand(SocketMessage socketMessage)
        {
            var author = (SocketGuildUser)socketMessage.Author;
            this.IsRequestedByAdmin = author.Roles.Any(r => r.Permissions.Administrator);
        }
    }
}
