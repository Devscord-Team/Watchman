using Devscord.DiscordFramework.Commands;
using Devscord.DiscordFramework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Users.BotCommands.Warns
{
    public class WarnsCommand : IBotCommand
    {
        [Optional]
        [UserMention]
        public ulong User { get; set; }
    }
}
