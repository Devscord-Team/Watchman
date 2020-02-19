using Watchman.Cqrs;

namespace Watchman.DomainModel.Users.Queries
{
    public class GetMuteEventsQuery : IQuery<GetMuteEventsQueryResult>
    {
        public ulong ServerId { get; }

        public GetMuteEventsQuery(ulong serverId)
        {
            ServerId = serverId;
        }
    }
}
