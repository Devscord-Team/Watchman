using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Responses.BotCommands
{
    public class AddResponseCommand : IBotCommand
    {
        [SingleWord]
        public string OnEvent { get; set; }
        [Text]
        public string Message { get; set; }
    }
}
