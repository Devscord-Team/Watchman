using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Help.BotCommands
{
    public class HelpCommand : IBotCommand
    {
        [Bool]
        [Optional]
        public bool Json { get; set; }
    }
}
