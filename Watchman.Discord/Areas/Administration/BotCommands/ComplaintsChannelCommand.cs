using Devscord.DiscordFramework.Commands;
using Devscord.DiscordFramework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Administration.BotCommands
{
    public class ComplaintsChannelCommand : IBotCommand
    {
        [Optional]
        [SingleWord]
        public string Name { get; set; }
    }
}
