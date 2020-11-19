using System.Collections.Generic;
using System.Linq;
using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Messages.Queries.Handlers
{
    public class GetMessagesQueryHandler : PaginationMessagesQueryHandler, IQueryHandler<GetMessagesQuery, GetMessagesQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetMessagesQueryHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public GetMessagesQueryResult Handle(GetMessagesQuery query)
        {
            using var session = this._sessionFactory.CreateMongo();
            var messages = session.Get<Message>();
            if (query.ServerId != 0)
            {
                messages = this.TakeOnlyFromOneServer(query.ServerId, messages);
            }
            if (query.ChannelId != 0)
            {
                messages = this.TakeOnlyFromChannel(query.ChannelId, messages);
            }
            if (query.UserId != 0)
            {
                messages = this.TakeOnlyForUser(query.UserId, messages);
            }
            var paginated = this.Paginate(query, messages);
            return new GetMessagesQueryResult(paginated);
        }

        private IEnumerable<Message> TakeOnlyFromOneServer(ulong serverId, IEnumerable<Message> messages)
        {
            return messages.Where(x => x.Server.Id == serverId);
        }

        private IEnumerable<Message> TakeOnlyFromChannel(ulong channelId, IEnumerable<Message> messages)
        {
            return messages.Where(x => x.Channel.Id == channelId);
        }

        private IEnumerable<Message> TakeOnlyForUser(ulong? userId, IEnumerable<Message> messages)
        {
            return messages.Where(x => x.Author.Id == userId);
        }
    }
}
