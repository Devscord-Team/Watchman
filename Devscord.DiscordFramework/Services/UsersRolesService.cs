using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Services
{
    public class UsersRolesService
    {
        public const string MUTED_ROLE_NAME = "muted";

        public async Task<UserRole> CreateNewRole(DiscordServerContext server, NewUserRole userRole)
        {
            return await Server.CreateNewRole(userRole, server);
        }

        public UserRole GetRoleByName(string name, DiscordServerContext server)
        {
            return Server.GetRoles(server.Id).FirstOrDefault(x => x.Name == name);
        }

        public IEnumerable<UserRole> GetRoles(DiscordServerContext server)
        {
            return Server.GetRoles(server.Id);
        }
    }
}
