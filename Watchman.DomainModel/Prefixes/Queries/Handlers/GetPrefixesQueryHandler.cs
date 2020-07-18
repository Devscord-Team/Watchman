using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.ServerPrefixes.Queries.Handlers
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
            using var session = this._sessionFactory.Create();
            var prefixes = session.Get<ServerPrefixes>();
            return new GetPrefixesQueryResult(prefixes);
        }
    }
}
