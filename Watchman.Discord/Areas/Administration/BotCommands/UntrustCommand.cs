using Devscord.DiscordFramework.Commands;
using Devscord.DiscordFramework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Administration.BotCommands
{
    public class UntrustCommand : IBotCommand
    {
        [Text]
        public string Role { get; set; }
    }
}
