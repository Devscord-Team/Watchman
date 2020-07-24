using System;
using System.Collections.Generic;
using System.Linq;
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
            using var session = this._sessionFactory.Create();
            var prefixes = session.Get<ServerPrefixes>().FirstOrDefault(x => x.ServerId == query.ServerId);
            return new GetPrefixesQueryResult(prefixes);
        }
    }
}
