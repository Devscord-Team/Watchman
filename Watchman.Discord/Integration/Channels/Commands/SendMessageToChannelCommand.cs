using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.Discord.Integration.Channels.Commands
{
    public class SendMessageToChannelCommand : ICommand
    {
        public ulong GuildId { get; private set; }
        public ulong ChannelId { get; private set; }
        public string Message { get; private set; }

        public SendMessageToChannelCommand(ulong guildId, ulong channelId, string message)
        {
            GuildId = guildId;
            ChannelId = channelId;
            Message = message;
        }
    }
}
