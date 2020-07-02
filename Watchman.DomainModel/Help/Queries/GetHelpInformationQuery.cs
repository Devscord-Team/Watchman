using Watchman.Cqrs;

namespace Watchman.DomainModel.Help.Queries
{
    public class GetHelpInformationQuery : IQuery<GetHelpInformationQueryResult>
    {
        public ulong ServerId { get; }

        public GetHelpInformationQuery(ulong serverId)
        {
            this.ServerId = serverId;
        }
    }
}
