using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Services
{
    public interface IUsersRolesService
    {
        Task<UserRole> CreateNewRole(DiscordServerContext server, NewUserRole userRole);
        UserRole GetRoleByName(string name, DiscordServerContext server);
        IEnumerable<UserRole> GetRoles(DiscordServerContext server);
        UserRole GetRole(ulong roleId, ulong serverId);
    }

    public class UsersRolesService : IUsersRolesService
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

        public UserRole GetRole(ulong roleId, ulong serverId)
        {
            return Server.GetRole(roleId, serverId);
        }
    }
}
