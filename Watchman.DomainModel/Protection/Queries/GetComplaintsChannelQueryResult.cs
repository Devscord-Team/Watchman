using Watchman.Cqrs;

namespace Watchman.DomainModel.Protection.Queries
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
