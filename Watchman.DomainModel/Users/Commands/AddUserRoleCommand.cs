using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Users.Commands
{
    public class AddUserRoleCommand : ICommand
    {
        public long UserId { get; private set; }
        public long ChannellId { get; private set; }
        public long RoleId { get; private set; }

        public AddUserRoleCommand(long userId, long channelId, long roleId)
        {
            this.UserId = userId;
            this.ChannellId = channelId;
            this.RoleId = roleId;
        }
    }
}
