using System.Linq;
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
            if (query.ServerId != 0)
            {
                messages = TakeOnlyFromOneServer(query.ServerId, messages);
            }

            var paginated = this.Paginate(query, messages);
            return new GetMessagesQueryResult(paginated);
        }

        private IQueryable<Message> TakeOnlyFromOneServer(ulong serverId, IQueryable<Message> messages)
        {
            return messages.Where(x => x.Server.Id == serverId);
        }
    }
}
