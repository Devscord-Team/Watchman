using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.ServerPrefixes.Queries
{
    public class GetPrefixesQueryResult : IQueryResult
    {
        public ServerPrefixes Prefixes { get; }

        public GetPrefixesQueryResult(ServerPrefixes prefixes)
        {
            this.Prefixes = prefixes;
        }
    }
}
