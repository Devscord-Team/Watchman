using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using System.Collections.Generic;
using System.Linq;
using Watchman.DomainModel.DiscordServer;

namespace Watchman.Discord.Areas.Users.Services
{
    public class RolesService
    {
        private readonly UsersService _usersService;
        private readonly UsersRolesService _usersRolesService;

        public RolesService(UsersService usersService, UsersRolesService usersRolesService)
        {
            this._usersService = usersService;
            _usersRolesService = usersRolesService;
        }

        public void AddRoleToUser(IEnumerable<string> safeRoles, MessagesService messagesService, Contexts contexts, IEnumerable<string> commandRoles)
        {
            var allRoleNames = _usersRolesService.GetAllRoleNames(contexts.Server);
            var userRoles = contexts.User.Roles.Select(x => x.Name);

            foreach (var role in commandRoles)
            {
                if (!allRoleNames.Contains(role) || !safeRoles.Contains(role))
                {
                    messagesService.SendResponse(x => x.RoleNotFoundOrIsNotSafe(contexts, role), contexts);
                    continue;
                }
                if (userRoles.Contains(role))
                {
                    messagesService.SendResponse(x => x.RoleIsInUserAlready(contexts, role), contexts);
                    continue;
                }
                var serverRole = _usersRolesService.GetRoleByName(role, contexts.Server);
                _usersService.AddRole(serverRole, contexts.User, contexts.Server).Wait();
                messagesService.SendResponse(x => x.RoleAddedToUser(contexts, role), contexts);
            }
        }

        public void DeleteRoleFromUser(IEnumerable<Role> safeRoles, MessagesService messagesService, Contexts contexts, string commandRole)
        {
            var role = safeRoles.FirstOrDefault(x => x.Name == commandRole);
            if (role == null)
            {
                messagesService.SendResponse(x => x.RoleNotFoundOrIsNotSafe(contexts, commandRole), contexts);
                return;
            }
            if (contexts.User.Roles.All(x => x.Name != role.Name))
            {
                messagesService.SendResponse(x => x.RoleNotFoundInUser(contexts, commandRole), contexts);
                return;
            }
            var serverRole = _usersRolesService.GetRoleByName(commandRole, contexts.Server);
            _usersService.RemoveRole(serverRole, contexts.User, contexts.Server).Wait();
            messagesService.SendResponse(x => x.RoleRemovedFromUser(contexts, commandRole), contexts);
        }
    }
}
