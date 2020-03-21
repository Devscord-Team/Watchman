using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Messages.Commands
{
    public class AddMessagesCommand : ICommand
    {
        public IEnumerable<Message> Messages { get; private set; }
        public ulong ChannelId { get; private set; }

        public AddMessagesCommand(IEnumerable<Message> messages, ulong channelId)
        {
            Messages = messages;
            ChannelId = channelId;
        }
    }
}
