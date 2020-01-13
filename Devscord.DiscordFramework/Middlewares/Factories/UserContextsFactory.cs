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
            var roles = user.Roles.Select(x =>
            {
                var permissions = x.Permissions.ToList().Select(x => (Permission) x);
                return new UserRole(x.Id, x.Name, permissions);
            });

            var avatarUrl = user.GetAvatarUrl(ImageFormat.Png, 2048);
            return new UserContext(user.Id, user.ToString(), roles, avatarUrl, user.Mention);
        }
    }
}
