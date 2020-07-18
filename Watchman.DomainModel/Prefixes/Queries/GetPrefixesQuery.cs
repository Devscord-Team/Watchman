using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.ServerPrefixes.Queries
{
    public class GetPrefixesQuery : IQuery<GetPrefixesQueryResult>
    {
        public ulong ServerId { get; }

        public GetPrefixesQuery(ulong serverId)
        {
            this.ServerId = serverId;
        }
    }
}
