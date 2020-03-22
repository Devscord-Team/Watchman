using Watchman.Cqrs;
using Watchman.DomainModel.Commons.Queries.Handlers;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Messages.Queries.Handlers
{
    public class GetUserMessagesQueryHandler : PaginationQueryHandler, IQueryHandler<GetUserMessagesQuery, GetMessagesQueryResult>
    {
        private readonly GetMessagesQueryHandler getMessagesQueryHandler;

        public GetUserMessagesQueryHandler(ISessionFactory sessionFactory)
        {
            getMessagesQueryHandler = new GetMessagesQueryHandler(sessionFactory);
        }

        public GetMessagesQueryResult Handle(GetUserMessagesQuery query)
        {
            return getMessagesQueryHandler.Handle(query);
        }
    }
}
