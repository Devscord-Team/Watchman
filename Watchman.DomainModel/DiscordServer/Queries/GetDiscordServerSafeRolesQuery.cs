using Watchman.Cqrs;

namespace Watchman.DomainModel.DiscordServer.Queries
{
    public class GetDiscordServerSafeRolesQuery : IQuery<GetDiscordServerSafeRolesQueryResult>
    {
        public ulong ServerId { get; }

        public GetDiscordServerSafeRolesQuery(ulong serverId)
        {
            this.ServerId = serverId;
        }
    }
}
