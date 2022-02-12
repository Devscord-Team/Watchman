using Devscord.DiscordFramework.Commands;
using Devscord.DiscordFramework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Responses.BotCommands
{
    public class RemoveResponseCommand : IBotCommand
    {
        [SingleWord]
        public string OnEvent { get; set; }
    }
}
