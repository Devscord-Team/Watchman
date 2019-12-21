using System.Linq;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Responses.Queries.Handlers
{
    public class GetResponsesQueryHandler : IQueryHandler<GetResponsesQuery, GetResponsesQueryResult>
    {
        private readonly ISessionFactory sessionFactory;

        public GetResponsesQueryHandler(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public GetResponsesQueryResult Handle(GetResponsesQuery query)
        {
            using (var session = sessionFactory.Create())
            {
                var responses = session.Get<Response>().ToList();
                return new GetResponsesQueryResult(responses);
            }
        }
    }
}
