using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Middlewares.Factories;

namespace Devscord.DiscordFramework.Services
{
    public class UsersRolesService
    {
        public const string MUTED_ROLE_NAME = "muted";

        public UserRole CreateNewRole(Contexts contexts, UserRole userRole)
        {
            return Server.CreateNewRole(userRole, contexts.Server).Result;
        }

        public UserRole GetRoleByName(string name, DiscordServerContext server)
        {
            var role = Server.GetRoles(server.Id).FirstOrDefault(x => x.Name == name);

            return role == null ? default : new UserRoleFactory().Create(role);
        }

        public IEnumerable<UserRole> GetRoles(DiscordServerContext server)
        {
            var roles = Server.GetRoles(server.Id).ToList();

            if (!roles.Any())
                return default;

            var userRoles = roles.Select(x => new UserRoleFactory().Create(x));
            return userRoles;
        }
    }
}
