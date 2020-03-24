using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.Discord.Integration.Channels.Commands
{
    public class SendMessageToChannelCommand : ICommand
    {
        public ulong ChannelId { get; private set; }
        public string Message { get; private set; }

        public SendMessageToChannelCommand(ulong channelId, string message)
        {
            ChannelId = channelId;
            Message = message;
        }
    }
}
