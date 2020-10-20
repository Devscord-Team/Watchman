using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Responses.BotCommands
{
    public class ResponsesCommand : IBotCommand
    {
        [Bool]
        public bool Default { get; set; }
        [Bool]
        public bool Custom { get; set; }
    }
}
