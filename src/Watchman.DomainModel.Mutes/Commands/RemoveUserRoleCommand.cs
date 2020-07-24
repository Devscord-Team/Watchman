using Watchman.Cqrs;

namespace Watchman.DomainModel.Mutes.Commands
{
    public class RemoveUserRoleCommand : ICommand
    {
        public long UserId { get; private set; }
        public long ChannellId { get; private set; }
        public long RoleId { get; private set; }

        public RemoveUserRoleCommand(long userId, long channelId, long roleId)
        {
            this.UserId = userId;
            this.ChannellId = channelId;
            this.RoleId = roleId;
        }
    }
}
