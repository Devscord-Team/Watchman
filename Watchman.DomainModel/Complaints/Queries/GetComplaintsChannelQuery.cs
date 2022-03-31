using Watchman.Cqrs;

namespace Watchman.DomainModel.Complaints.Queries
{
    public class GetComplaintsChannelQuery : IQuery<GetComplaintsChannelQueryResult>
    {
        public ulong ServerId { get; }

        public GetComplaintsChannelQuery(ulong serverId)
        {
            this.ServerId = serverId;
        }
    }
}
