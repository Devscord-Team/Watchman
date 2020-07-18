using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Prefixes.Queries
{
    public class GetPrefixesQueryResult : IQueryResult
    {
        public IEnumerable<Prefix> Prefixes { get; }

        public GetPrefixesQueryResult(IEnumerable<Prefix> prefixes)
        {
            this.Prefixes = prefixes;
        }
    }
}
