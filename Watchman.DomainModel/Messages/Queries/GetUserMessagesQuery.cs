namespace Watchman.DomainModel.Messages.Queries
{
    public class GetUserMessagesQuery : GetMessagesQuery
    {
        public ulong UserId { get; private set; }

        public GetUserMessagesQuery(ulong serverId, ulong userId) : base(serverId)
        {
            UserId = userId;
        }
    }
}
