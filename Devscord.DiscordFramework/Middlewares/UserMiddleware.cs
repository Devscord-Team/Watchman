using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares
{
    public class UserMiddleware : IMiddleware<UserContext>
    {
        public UserContext Process(SocketMessage data)
        {
            var user = (SocketGuildUser)data.Author;
            var roles = user.Roles.Select(x => x.Name);
            var avatarUrl = user.GetAvatarUrl(ImageFormat.Png, 2048);
            return new UserContext(user.Id, user.ToString(), roles, avatarUrl);
        }
    }
}
