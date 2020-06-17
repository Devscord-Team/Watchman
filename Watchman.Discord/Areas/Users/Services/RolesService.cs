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
            var userRoleNames = contexts.User.Roles.Select(x => x.Name).ToList();
            var safeRoleNames = safeRoles.Select(x => x.Name).ToList();
            foreach (var role in commandRoles)
            {
                if (userRoleNames.Contains(role))
                {
                    messagesService.SendResponse(x => x.RoleIsInUserAlready(contexts, role));
                    continue;
                }
                var serverRole = this._usersRolesService.GetRoleByName(role, contexts.Server);
                if (serverRole == null || !safeRoleNames.Contains(role))
                {
                    messagesService.SendResponse(x => x.RoleNotFoundOrIsNotSafe(contexts, role));
                    continue;
                }
                this._usersService.AddRole(serverRole, contexts.User, contexts.Server).Wait();
                messagesService.SendResponse(x => x.RoleAddedToUser(contexts, role));
            }
        }

        public void DeleteRoleFromUser(IEnumerable<Role> safeRoles, MessagesService messagesService, Contexts contexts, string commandRole)
        {
            var role = safeRoles.FirstOrDefault(x => x.Name == commandRole);
            if (role == null)
            {
                messagesService.SendResponse(x => x.RoleNotFoundOrIsNotSafe(contexts, commandRole));
                return;
            }
            if (contexts.User.Roles.All(x => x.Name != role.Name))
            {
                messagesService.SendResponse(x => x.RoleNotFoundInUser(contexts, commandRole));
                return;
            }
            var serverRole = this._usersRolesService.GetRoleByName(commandRole, contexts.Server);
            this._usersService.RemoveRole(serverRole, contexts.User, contexts.Server).Wait();
            messagesService.SendResponse(x => x.RoleRemovedFromUser(contexts, commandRole));
        }

        public async Task SetRolesAsSafe(Contexts contexts, IEnumerable<string> commandRoles, bool setAsSafe)
        {
            var safeRolesQuery = new GetDiscordServerSafeRolesQuery(contexts.Server.Id);
            var safeRoles = this._queryBus.Execute(safeRolesQuery).SafeRoles.ToList();
            var messageService = this._messagesServiceFactory.Create(contexts);

            foreach (var roleName in commandRoles)
            {
                var serverRole = this._usersRolesService.GetRoleByName(roleName, contexts.Server);
                if (serverRole == null)
                {
                    await messageService.SendResponse(x => x.RoleNotFoundOrIsNotSafe(contexts, roleName));
                    continue;
                }
                var settingsWasChanged = setAsSafe
                    ? await TryToSetAsSafe(safeRoles, roleName, contexts)
                    : await TryToSetAsUnsafe(safeRoles, roleName, contexts);
                if (settingsWasChanged)
                {
                    await messageService.SendResponse(x => x.RoleSettingsChanged(roleName));
                }
            }
        }

        private async Task<bool> TryToSetAsSafe(IEnumerable<Role> safeRoles, string roleName, Contexts contexts)
        {
            if (safeRoles.Any(x => x.Name == roleName))
            {
                var messageService = this._messagesServiceFactory.Create(contexts);
                await messageService.SendResponse(x => x.RoleIsSafeAlready(roleName));
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
                await messageService.SendResponse(x => x.RoleIsUnsafeAlready(roleName));
                return false;
            }
            await this._commandBus.ExecuteAsync(new SetRoleAsUnsafeCommand(roleName, contexts.Server.Id));
            return true;
        }
    }
}
