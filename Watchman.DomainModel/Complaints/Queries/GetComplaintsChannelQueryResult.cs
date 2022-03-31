using Watchman.Cqrs;
using Watchman.DomainModel.Complaints;

namespace Watchman.DomainModel.Complaints.Queries
{
    public class GetComplaintsChannelQueryResult : IQueryResult
    {
        public ComplaintsChannel ComplaintsChannel { get; }

        public GetComplaintsChannelQueryResult(ComplaintsChannel complaintsChannel)
        {
            this.ComplaintsChannel = complaintsChannel;
        }
    }
}
