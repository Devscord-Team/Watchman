using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Commons.Exceptions;
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
        UserRole GetMuteRole(DiscordServerContext server);
    }

    public class UsersRolesService : IUsersRolesService
    {
        public const string MUTED_ROLE_NAME = "muted";//todo should be in Domain

        public Task<UserRole> CreateNewRole(DiscordServerContext server, NewUserRole userRole)
            => Server.CreateNewRole(userRole, server);

        public UserRole GetRoleByName(string name, DiscordServerContext server)
            => Server.GetRoles(server.Id).FirstOrDefault(x => x.Name == name);

        public IEnumerable<UserRole> GetRoles(DiscordServerContext server)
            => Server.GetRoles(server.Id);

        public UserRole GetRole(ulong roleId, ulong serverId)
            => Server.GetRole(roleId, serverId);

        public UserRole GetMuteRole(DiscordServerContext server)
            => this.GetRoleByName(MUTED_ROLE_NAME, server)
                ?? throw new RoleNotFoundException(MUTED_ROLE_NAME);
    }
}
