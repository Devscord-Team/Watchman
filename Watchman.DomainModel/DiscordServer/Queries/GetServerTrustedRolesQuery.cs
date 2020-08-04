using Watchman.Cqrs;

namespace Watchman.DomainModel.DiscordServer.Queries
{
    public class GetServerTrustedRolesQuery : IQuery<GetServerTrustedRolesQueryResult>
    {
        public ulong ServerId { get; }

        public GetServerTrustedRolesQuery(ulong serverId)
        {
            this.ServerId = serverId;
        }
    }
}
