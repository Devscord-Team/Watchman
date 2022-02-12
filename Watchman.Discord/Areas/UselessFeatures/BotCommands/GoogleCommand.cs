using Devscord.DiscordFramework.Commands;
using Devscord.DiscordFramework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.UselessFeatures.BotCommands
{
    public class GoogleCommand : IBotCommand
    {
        [Text]
        public string Search { get; set; }
    }
}
