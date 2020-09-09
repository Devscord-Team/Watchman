using Watchman.Cqrs;

namespace Watchman.DomainModel.Settings
{
    public class GetConfigurationInformationQuery :IQuery<GetConfigurationInformationQueryResult>
    {
        public ulong ServerId { get; }

        public GetConfigurationInformationQuery(ulong serverId)
        {
            this.ServerId = serverId;
        }
    }
}