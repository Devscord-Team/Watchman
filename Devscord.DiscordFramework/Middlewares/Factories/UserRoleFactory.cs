using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord;
using System.Linq;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    public interface IUserRoleFactory
    {
        UserRole Create(IRole socketRole);
    }

    public class UserRoleFactory
    {
        public UserRole Create(IRole socketRole)
        {
            var permissions = socketRole.Permissions.ToList().Select(x => (Permission) x).ToList();
            return new UserRole(socketRole.Id, socketRole.Name, socketRole.Guild.Id, permissions);
        }
    }
}
