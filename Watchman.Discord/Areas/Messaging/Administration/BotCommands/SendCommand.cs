using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Messaging.Administration.BotCommands
{
    public class SendCommand : IBotCommand
    {
        [ChannelMention]
        public ulong Channel { get; set; }

        [Text]
        public string Message { get; set; }
    }
}
