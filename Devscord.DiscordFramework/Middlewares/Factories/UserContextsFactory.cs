using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    internal class UserContextsFactory : IContextFactory<IUser, UserContext>
    {
        private readonly UserRoleFactory _userRoleFactory;

        public UserContextsFactory()
        {
            this._userRoleFactory = new UserRoleFactory();
        }

        public UserContext Create(IUser user)
        {
            var socketGuildUser = user as SocketGuildUser;
            var roles = socketGuildUser?.Roles.Select(x => this._userRoleFactory.Create(x)).ToList() ?? new List<UserRole>();
            var avatarUrl = user.GetAvatarUrl(ImageFormat.Png, 2048);
            var isOwner = socketGuildUser?.Guild.OwnerId == user.Id;

            return new UserContext(user.Id, user.ToString(), roles, avatarUrl, user.Mention, isOwner);
        }
    }
}
