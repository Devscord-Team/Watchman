using Watchman.Cqrs;

namespace Watchman.DomainModel.Users.Queries
{
    public class GetMuteEventsQuery : IQuery<GetMuteEventsQueryResult>
    {
        public ulong ServerId { get; }
        public bool TakeOnlyNotUnmuted { get; }
        public ulong? UserId { get; }

        public GetMuteEventsQuery(ulong serverId, bool takeOnlyNotUnmuted, ulong? userId = null)
        {
            this.ServerId = serverId;
            this.TakeOnlyNotUnmuted = takeOnlyNotUnmuted;
            this.UserId = userId;
        }
    }
}
