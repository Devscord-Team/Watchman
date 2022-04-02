using Watchman.Cqrs;

namespace Watchman.DomainModel.Help.Queries
{
    public class GetHelpInformationsQuery : IQuery<GetHelpInformationsQueryResult>
    {
        public ulong ServerId { get; }

        public GetHelpInformationsQuery(ulong serverId)
        {
            this.ServerId = serverId;
        }
    }
}
