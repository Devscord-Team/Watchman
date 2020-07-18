using Watchman.Cqrs;

namespace Watchman.DomainModel.Mutes.Queries
{
    public class GetMuteEventsQuery : IQuery<GetMuteEventsQueryResult>
    {
        public ulong ServerId { get; }

        public GetMuteEventsQuery(ulong serverId)
        {
            this.ServerId = serverId;
        }
    }
}
