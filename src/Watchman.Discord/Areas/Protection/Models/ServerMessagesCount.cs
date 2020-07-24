using System.Collections.Generic;
using System.Linq;
using Watchman.DomainModel.Messages;

namespace Watchman.Discord.Areas.Protection.Models
{
    public class ServerMessagesCount
    {
        public ulong ServerId { get; private set; }
        public List<(ulong UserId, int messagesQuantity)> UsersMessagesQuantity { get; private set; }

        public ServerMessagesCount(IEnumerable<Message> serverMessages, ulong serverId)
        {
            this.ServerId = serverId;
            this.UsersMessagesQuantity = serverMessages
                .GroupBy(x => x.Author.Id)
                .Select(x => (x.Key, x.Count()))
                .ToList();
        }
    }
}