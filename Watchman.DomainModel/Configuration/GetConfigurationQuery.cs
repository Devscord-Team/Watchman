using Watchman.Cqrs;

namespace Watchman.DomainModel.Configuration
{
    public class GetConfigurationQuery : IQuery<GetConfigurationQueryResult>
    {
        public ulong ServerId { get; }

        public GetConfigurationQuery(ulong serverId)
        {
            this.ServerId = serverId;
        }
    }
}