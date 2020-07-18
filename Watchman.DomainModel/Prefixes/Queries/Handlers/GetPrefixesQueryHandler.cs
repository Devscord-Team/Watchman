using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Prefixes.Queries.Handlers
{
    public class GetPrefixesQueryHandler : IQueryHandler<GetPrefixesQuery, GetPrefixesQueryResult>
    {
        public GetPrefixesQueryResult Handle(GetPrefixesQuery query)
        {
            throw new NotImplementedException();
        }
    }
}
