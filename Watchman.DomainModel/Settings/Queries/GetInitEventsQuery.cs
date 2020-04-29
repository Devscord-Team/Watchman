using Watchman.Cqrs;

namespace Watchman.DomainModel.Settings.Queries
{
    public class GetInitEventsQuery : IQuery<GetInitEventsQueryResults>
    {
        public ulong ServerId { get; set; }

        public GetInitEventsQuery(ulong serverId)
        {
            ServerId = serverId;
        }
    }
}
