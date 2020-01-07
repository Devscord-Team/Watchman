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

        public RolesService(UsersService usersService)
        {
            this._usersService = usersService;
        }

        public void AddRoleToUser(IEnumerable<Role> safeRoles, MessagesService messagesService, Contexts contexts, string commandRole)
        {
            var role = safeRoles.FirstOrDefault(x => x.Name == commandRole);
            if (role == null)
            {
                messagesService.SendResponse(x => x.RoleNotFoundOrIsNotSafe(contexts, commandRole), contexts);
                return;
            }
            if (contexts.User.Roles.Any(x => x.Name == role.Name))
            {
                messagesService.SendResponse(x => x.RoleIsInUserAlready(contexts, commandRole), contexts);
                return;
            }
            var serverRole = _usersService.GetRoleByName(commandRole, contexts.Server);
            _usersService.AddRole(serverRole, contexts.User, contexts.Server).Wait();
            messagesService.SendResponse(x => x.RoleAddedToUser(contexts, commandRole), contexts);
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
            var serverRole = _usersService.GetRoleByName(commandRole, contexts.Server);
            _usersService.RemoveRole(serverRole, contexts.User, contexts.Server).Wait();
            messagesService.SendResponse(x => x.RoleRemovedFromUser(contexts, commandRole), contexts);
        }
    }
}
