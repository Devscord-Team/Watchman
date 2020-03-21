using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord;
using Discord.WebSocket;

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
            var roles = socketGuildUser?.Roles.Select(x => _userRoleFactory.Create(x)) ?? new List<UserRole>();
            var avatarUrl = user.GetAvatarUrl(ImageFormat.Png, 2048);

            return new UserContext(user.Id, user.ToString(), roles, avatarUrl, user.Mention);
        }
    }
}
