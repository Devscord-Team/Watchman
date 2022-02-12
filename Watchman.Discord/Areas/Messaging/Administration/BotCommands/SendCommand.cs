using Devscord.DiscordFramework.Commands;
using Devscord.DiscordFramework.Commands.PropertyAttributes;

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
