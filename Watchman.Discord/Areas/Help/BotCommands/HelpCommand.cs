using Devscord.DiscordFramework.Commands;
using Devscord.DiscordFramework.Commands.PropertyAttributes;

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
