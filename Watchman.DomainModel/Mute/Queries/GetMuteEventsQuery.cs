using Watchman.Cqrs;

namespace Watchman.DomainModel.Mute.Queries
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
