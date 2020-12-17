using Watchman.Cqrs;

namespace Watchman.DomainModel.CustomCommands.Queries
{
    public class GetCustomCommandsQuery : IQuery<GetCustomCommandsQueryResult>
    {
        public ulong ServerId { get; }

        public GetCustomCommandsQuery()
        {
        }

        public GetCustomCommandsQuery(ulong serverId)
        {
            this.ServerId = serverId;
        }
    }
}
