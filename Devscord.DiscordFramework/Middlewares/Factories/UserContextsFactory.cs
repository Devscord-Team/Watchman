using System.Linq;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Middlewares.Factories;
using Discord;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares
{
    internal class UserContextsFactory : IContextFactory<SocketGuildUser, UserContext>
    {
        public UserContext Create(SocketGuildUser user)
        {
            var roles = user.Roles.Select(x => new UserRole(x.Id, x.Name));
            var avatarUrl = user.GetAvatarUrl(ImageFormat.Png, 2048);
            return new UserContext(user.Id, user.ToString(), roles, avatarUrl, user.Mention);
        }
    }
}
