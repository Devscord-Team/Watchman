using Watchman.Cqrs;
using Watchman.DomainModel.Commons.Queries.Handlers;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Messages.Queries.Handlers
{
    public class GetUserMessagesQueryHandler : PaginationQueryHandler, IQueryHandler<GetUserMessagesQuery, GetMessagesQueryResult>
    {
        private readonly GetMessagesQueryHandler _getMessagesQueryHandler;

        public GetUserMessagesQueryHandler(ISessionFactory sessionFactory)
        {
            _getMessagesQueryHandler = new GetMessagesQueryHandler(sessionFactory);
        }

        public GetMessagesQueryResult Handle(GetUserMessagesQuery query)
        {
            return _getMessagesQueryHandler.Handle(query);
        }
    }
}
