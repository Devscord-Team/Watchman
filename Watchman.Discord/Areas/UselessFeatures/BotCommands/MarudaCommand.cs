using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.UselessFeatures.BotCommands
{
    public class MarudaCommand : IBotCommand
    {
        [UserMention]
        public ulong User { get; set; }
        [ChannelMention]
        public ulong Channel { get; set; }
    }
}
