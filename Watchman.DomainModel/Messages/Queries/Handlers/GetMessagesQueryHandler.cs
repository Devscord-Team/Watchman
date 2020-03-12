using Watchman.Cqrs;
using Watchman.DomainModel.Commons.Queries.Handlers;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Messages.Queries.Handlers
{
    public class GetMessagesQueryHandler : PaginationQueryHandler, IQueryHandler<GetMessagesQuery, GetMessagesQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetMessagesQueryHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public GetMessagesQueryResult Handle(GetMessagesQuery query)
        {
            using var session = _sessionFactory.Create();
            var messages = session.Get<Message>();
            var paginated = this.Paginate(query, messages);
            return new GetMessagesQueryResult(paginated);
        }
    }
}
