using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Administration.BotCommands
{
    public class TrustCommand : IBotCommand
    {
        [SingleWord]
        public string RoleName { get; set; }
    }
}
