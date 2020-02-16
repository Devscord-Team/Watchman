using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Services
{
    public class UsersRolesService
    {
        public const string MUTED_ROLE_NAME = "muted";

        public UserRole CreateNewRole(Contexts contexts, NewUserRole userRole)
        {
            return Server.CreateNewRole(userRole, contexts.Server).Result;
        }

        public UserRole GetRoleByName(string name, DiscordServerContext server)
        {
            return Server.GetRoles(server.Id).FirstOrDefault(x => x.Name == name);
        }

        public IEnumerable<UserRole> GetRoles(DiscordServerContext server)
        {
            return Server.GetRoles(server.Id).ToList();
        }
    }
}
