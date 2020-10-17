using System.Linq;

using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Responses.Queries.Handlers
{
    public class GetResponseQueryHandler : IQueryHandler<GetResponseQuery, GetResponseQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetResponseQueryHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public GetResponseQueryResult Handle(GetResponseQuery query)
        {
            using var session = this._sessionFactory.CreateMongo();
            var response = session.Get<Response>().FirstOrDefault(x => x.ServerId == query.ServerId && x.OnEvent == query.OnEvent);
            return new GetResponseQueryResult(response);
        }
    }
}
