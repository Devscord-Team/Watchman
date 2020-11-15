using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Help.BotCommands
{
    public class HelpCommand : IBotCommand
    {
        [Bool]
        public bool Json { get; set; }

        [Optional]
        [Text]
        public string Command { get; set; }
    }
}
