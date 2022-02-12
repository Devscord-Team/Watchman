using System.Collections.Generic;
using Devscord.DiscordFramework.Commands;
using Devscord.DiscordFramework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Users.BotCommands
{
    public class RemoveRoleCommand : IBotCommand
    {
        [List]
        public List<string> Roles { get; set; }
    }
}
