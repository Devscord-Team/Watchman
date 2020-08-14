using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.UselessFeatures.BotCommands
{
    public class GoogleCommand : IBotCommand
    {
        [Text]
        public string Search { get; set; }
    }
}
