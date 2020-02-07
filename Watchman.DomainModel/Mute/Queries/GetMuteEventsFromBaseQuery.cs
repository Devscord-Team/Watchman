using Watchman.Cqrs;

namespace Watchman.DomainModel.Mute.Queries
{
    public class GetMuteEventsFromBaseQuery : IQuery<GetMuteEventsFromBaseQueryResult>
    {
        public ulong ServerId { get; }

        public GetMuteEventsFromBaseQuery(ulong serverId)
        {
            ServerId = serverId;
        }
    }
}
