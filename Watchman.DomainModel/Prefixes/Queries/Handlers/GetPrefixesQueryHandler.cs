using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Prefixes.Queries.Handlers
{
    public class GetPrefixesQueryHandler : IQueryHandler<GetPrefixesQuery, GetPrefixesQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetPrefixesQueryHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public GetPrefixesQueryResult Handle(GetPrefixesQuery query)
        {
            throw new NotImplementedException();
        }
    }
}
