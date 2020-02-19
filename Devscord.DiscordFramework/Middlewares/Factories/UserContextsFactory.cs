using System.Linq;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    internal class UserContextsFactory : IContextFactory<SocketGuildUser, UserContext>
    {
        private readonly UserRoleFactory _userRoleFactory;

        public UserContextsFactory()
        {
            this._userRoleFactory = new UserRoleFactory();
        }

        public UserContext Create(SocketGuildUser user)
        {
            var roles = user.Roles.Select(x => _userRoleFactory.Create(x));
            var avatarUrl = user.GetAvatarUrl(ImageFormat.Png, 2048);

            return new UserContext(user.Id, user.ToString(), roles, avatarUrl, user.Mention);
        }
    }
}
