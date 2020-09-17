using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Warns.Queries
{
    public class GetWarnEventsQuery : IQuery<GetWarnEventsQueryResults>
    {
        public ulong ServerId { get; }
        public ulong ReceiverId { get; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public GetWarnEventsQuery(ulong serverId, ulong receiverId, DateTime from, DateTime to)
        {
            this.ServerId = serverId;
            this.ReceiverId = receiverId;
            this.From = from;
            this.To = to;
        }
    }
}
