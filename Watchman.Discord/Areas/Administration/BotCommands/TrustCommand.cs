using Devscord.DiscordFramework.Commands;
using Devscord.DiscordFramework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Administration.BotCommands
{
    public class TrustCommand : IBotCommand
    {
        [Text]
        public string Role { get; set; }
    }
}
