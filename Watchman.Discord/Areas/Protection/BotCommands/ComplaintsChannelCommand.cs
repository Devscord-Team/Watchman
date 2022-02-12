using Devscord.DiscordFramework.Commands;
using Devscord.DiscordFramework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Protection.BotCommands
{
    public class ComplaintsChannelCommand : IBotCommand
    {
        [Optional]
        [SingleWord]
        public string Name { get; set; }
    }
}
