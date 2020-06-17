using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.DiscordServer;
using Watchman.DomainModel.DiscordServer.Commands;
using Watchman.DomainModel.DiscordServer.Queries;

namespace Watchman.Discord.Areas.Users.Services
{
    public class RolesService
    {
        private readonly UsersService _usersService;
        private readonly UsersRolesService _usersRolesService;
        private readonly IQueryBus _queryBus;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly ICommandBus _commandBus;

        public RolesService(UsersService usersService, UsersRolesService usersRolesService, IQueryBus queryBus, MessagesServiceFactory messagesServiceFactory, ICommandBus commandBus)
        {
            this._usersService = usersService;
            this._usersRolesService = usersRolesService;
            this._queryBus = queryBus;
            this._messagesServiceFactory = messagesServiceFactory;
            this._commandBus = commandBus;
        }

        public void AddRoleToUser(IEnumerable<Role> safeRoles, MessagesService messagesService, Contexts contexts, IEnumerable<string> commandRoles)
        {
            var userRoleNames = contexts.User.Roles.Select(x =>x.Name);
            var safeRoleNames = safeRoles.Select(x => x.Name);
            foreach (var role in commandRoles)
            {
                if (userRoleNames.Contains(role))
                {
                    messagesService.SendResponse(x => x.RoleIsInUserAlready(contexts, role), contexts);
                    continue;
                }
                var serverRole = this._usersRolesService.GetRoleByName(role, contexts.Server);
                if (serverRole == null || !safeRoleNames.Contains(role))
                {
                    messagesService.SendResponse(x => x.RoleNotFoundOrIsNotSafe(contexts, role), contexts);
                    continue;
                }
                this._usersService.AddRole(serverRole, contexts.User, contexts.Server).Wait();
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
            var serverRole = this._usersRolesService.GetRoleByName(commandRole, contexts.Server);
            this._usersService.RemoveRole(serverRole, contexts.User, contexts.Server).Wait();
            messagesService.SendResponse(x => x.RoleRemovedFromUser(contexts, commandRole), contexts);
        }

        public async Task SetRolesAsSafe(Contexts contexts, IEnumerable<string> commandRoles, bool setAsSafe)
        {
            var safeRolesQuery = new GetDiscordServerSafeRolesQuery(contexts.Server.Id);
            var safeRoles = this._queryBus.Execute(safeRolesQuery).SafeRoles;
            var messageService = this._messagesServiceFactory.Create(contexts);

            foreach (var roleName in commandRoles)
            {
                var serverRole = this._usersRolesService.GetRoleByName(roleName, contexts.Server);
                if (serverRole == null)
                {
                    await messageService.SendResponse(x => x.RoleNotFoundOrIsNotSafe(contexts, roleName), contexts);
                    continue;
                }
                var settingsWasChanged = setAsSafe
                    ? await TryToSetAsSafe(safeRoles, roleName, contexts)
                    : await TryToSetAsUnsafe(safeRoles, roleName, contexts);
                if (settingsWasChanged)
                {
                    await messageService.SendResponse(x => x.RoleSettingsChanged(roleName), contexts);
                }
            }
        }

        private async Task<bool> TryToSetAsSafe(IEnumerable<Role> safeRoles, string roleName, Contexts contexts)
        {
            if (safeRoles.Any(x => x.Name == roleName))
            {
                var messageService = this._messagesServiceFactory.Create(contexts);
                await messageService.SendResponse(x => x.RoleIsSafeAlready(roleName), contexts);
                return false;
            }
            await this._commandBus.ExecuteAsync(new SetRoleAsSafeCommand(roleName, contexts.Server.Id));
            return true;
        }

        private async Task<bool> TryToSetAsUnsafe(IEnumerable<Role> safeRoles, string roleName, Contexts contexts)
        {
            if (safeRoles.All(x => x.Name != roleName))
            {
                var messageService = this._messagesServiceFactory.Create(contexts);
                await messageService.SendResponse(x => x.RoleIsUnsafeAlready(roleName), contexts);
                return false;
            }
            await this._commandBus.ExecuteAsync(new SetRoleAsUnsafeCommand(roleName, contexts.Server.Id));
            return true;
        }
    }
}
