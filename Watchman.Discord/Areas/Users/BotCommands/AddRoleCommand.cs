using System.Collections.Generic;
using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Users.BotCommands
{
    public class AddRoleCommand : IBotCommand
    {
        [List]
        public List<string> RolesNames { get; set; }
    }
}
