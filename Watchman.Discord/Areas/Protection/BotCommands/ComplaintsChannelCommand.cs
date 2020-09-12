using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Protection.BotCommands
{
    public class ComplaintsChannelCommand : IBotCommand
    {
        [Optional]
        [SingleWord]
        public string Name { get; set; }
    }
}
