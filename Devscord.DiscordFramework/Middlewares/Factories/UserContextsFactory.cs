using System.Linq;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    internal class UserContextsFactory : IContextFactory<SocketGuildUser, UserContext>
    {
        public UserContext Create(SocketGuildUser user)
        {
            var userRoleFactory = new UserRoleFactory();

            var roles = user.Roles.Select(x => userRoleFactory.Create(x));
            var avatarUrl = user.GetAvatarUrl(ImageFormat.Png, 2048);

            return new UserContext(user.Id, user.ToString(), roles, avatarUrl, user.Mention);
        }
    }
}
