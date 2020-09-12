using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Warns.Queries
{
    public class GetWarnEventsQuery : IQuery<GetWarnEventsQueryResults>
    {
        public ulong ServerId { get; }
        public ulong UserId { get; }
        public DateTime From { get; set; }

        public GetWarnEventsQuery(ulong serverId, ulong userId)
        {
            this.ServerId = serverId;
            this.UserId = userId;
        }
    }
}
