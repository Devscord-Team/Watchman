using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Messages.Queries
{
    public class GetMessagesQueryResult : IQueryResult
    {
        public IEnumerable<Message> Messages { get; private set; }

        public GetMessagesQueryResult(IEnumerable<Message> messages)
        {
            this.Messages = messages;
        }
    }
}
