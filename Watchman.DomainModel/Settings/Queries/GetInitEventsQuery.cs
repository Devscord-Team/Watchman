using Watchman.Cqrs;

namespace Watchman.DomainModel.Settings.Queries
{
    public class GetInitEventsQuery : IQuery<GetInitEventsQueryResults>
    {
        public ulong ServerId { get; private set; }

        public GetInitEventsQuery(ulong serverId) => this.ServerId = serverId;
    }
}
