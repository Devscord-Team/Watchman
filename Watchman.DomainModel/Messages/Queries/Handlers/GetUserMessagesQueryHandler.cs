using Watchman.Cqrs;
using Watchman.DomainModel.Commons.Queries.Handlers;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Messages.Queries.Handlers
{
    public class GetUserMessagesQueryHandler : PaginationQueryHandler, IQueryHandler<GetUserMessagesQuery, GetMessagesQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetUserMessagesQueryHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public GetMessagesQueryResult Handle(GetUserMessagesQuery query)
        {
            return new GetMessagesQueryHandler(_sessionFactory).Handle(query);
        }
    }
}
