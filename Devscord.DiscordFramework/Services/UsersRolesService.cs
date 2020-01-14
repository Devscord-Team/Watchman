using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Services
{
    public class UsersRolesService
    {
        public Task CreateNewRole(Contexts contexts, UserRole userRole)
        {
            return Server.CreateNewRole(userRole, contexts.Server);
        }

        public UserRole GetRoleByName(string name, DiscordServerContext server)
        {
            var role = Server.GetRoles(server.Id).FirstOrDefault(x => x.Name == name);
            if(role == null)
            {
                return default;
            }
            var permissions = role.Permissions.ToList().Select(x => (Permission)x);
            return new UserRole(role.Id, role.Name, permissions);
        }

        public IEnumerable<UserRole> GetRoles(DiscordServerContext server)
        {
            var roles = Server.GetRoles(server.Id);

            var userRoles = roles.Select(x =>
            {
                var perms = x.Permissions.ToList().Select(x => (Permission)x);
                return new UserRole(x.Id, x.Name, perms);
            });
            return userRoles;
        }
    }
}
