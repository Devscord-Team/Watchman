using System.Linq;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    public class UserRoleFactory
    {
        public UserRole Create(IRole socketRole)
        {
            var permissions = socketRole.Permissions.ToList().Select(x => (Permission) x).ToList();
            return new UserRole(socketRole.Id, socketRole.Name, permissions);
        }
    }
}
