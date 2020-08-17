using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Users.BotCommands.Warns
{
    public class WarnsCommand : IBotCommand
    {
        [Optional]
        [UserMention]
        public ulong User { get; set; }
    }
}
