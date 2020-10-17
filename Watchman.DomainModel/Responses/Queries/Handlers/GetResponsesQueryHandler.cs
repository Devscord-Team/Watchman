using System.Linq;
using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Responses.Queries.Handlers
{
    public class GetResponsesQueryHandler : IQueryHandler<GetResponsesQuery, GetResponsesQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetResponsesQueryHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public GetResponsesQueryResult Handle(GetResponsesQuery query)
        {
            using var session = this._sessionFactory.CreateMongo();
            var responses = session.Get<Response>().ToList();
            return new GetResponsesQueryResult(responses);
        }
    }
}
