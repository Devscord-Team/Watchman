using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Messages.Queries
{
    public class GetMessagesQueryResult : IQueryResult
    {
        public IEnumerable<Message> Messages { get; private set; }

        public GetMessagesQueryResult(IEnumerable<Message> messages)
        {
            Messages = messages;
        }
    }
}
