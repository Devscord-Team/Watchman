using System.Linq;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.Rest;
using Discord.WebSocket;
using Watchman.Common.Models;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    public class UserRoleFactory
    {
        public UserRole Create(SocketRole socketRole)
        {
            var permissions = socketRole.Permissions.ToList().Select(x => (Permission) x).ToList();
            return new UserRole(socketRole.Id, socketRole.Name, permissions);
        }

        public UserRole Create(RestRole restRole)
        {
            var permissions = restRole.Permissions.ToList().Select(x => (Permission) x).ToList();
            return new UserRole(restRole.Id, restRole.Name, permissions);
        }
    }
}
