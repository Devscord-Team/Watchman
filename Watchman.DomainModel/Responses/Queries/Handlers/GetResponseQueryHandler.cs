using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Responses.Queries.Handlers
{
    public class GetResponseQueryHandler : IQueryHandler<GetResponseQuery, GetResponseQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetResponseQueryHandler(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }
        public GetResponseQueryResult Handle(GetResponseQuery query)
        {
            using var session = _sessionFactory.Create();
            var response = session.Get<Response>().FirstOrDefault(x => x.ServerId == query.ServerId && x.OnEvent.ToLowerInvariant() == query.OnEvent.ToLowerInvariant());
            return new GetResponseQueryResult(response);
        }
    }
}
