using System.Collections.Generic;
using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Users.BotCommands
{
    public class AddRoleCommand : IBotCommand // todo: change to AddRoleCommand when two words names of IBotCommand will be implemented
    {
        [List]
        public List<string> Roles { get; set; }
    }
}
