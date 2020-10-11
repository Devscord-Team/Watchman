using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Common.Models;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Warns.Queries
{
    public class GetWarnEventsQuery : IQuery<GetWarnEventsQueryResults>
    {
        public ulong ServerId { get; }
        public ulong ReceiverId { get; }
        public TimeRange TimeRange { get; }

        public GetWarnEventsQuery(ulong serverId, ulong receiverId, TimeRange timeRange)
        {
            this.ServerId = serverId;
            this.ReceiverId = receiverId;
            this.TimeRange = timeRange;
        }
    }
}
