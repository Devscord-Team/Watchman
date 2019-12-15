using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Help.Queries
{
    public class GetHelpInformationQuery : IQuery<GetHelpInformationQueryResult>
    {
        public ISession Session { get; }
        public ulong ServerId { get; }

        public GetHelpInformationQuery(ISession session, ulong serverId)
        {
            this.Session = session;
            this.ServerId = serverId;
        }
    }
}
