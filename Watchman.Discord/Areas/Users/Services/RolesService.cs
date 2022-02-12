using Devscord.DiscordFramework.Commands.Responses;
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

        public async Task AddRoleToUser(IEnumerable<SafeRole> safeRoles, Contexts contexts, IReadOnlyCollection<string> rolesToAdd)
        {
            var messagesService = this._messagesServiceFactory.Create(contexts);
            var safeRolesIds = safeRoles.Select(x => x.RoleId).ToList();
            var addedRoles = new List<string>();

            foreach (var role in rolesToAdd)
            {
                if (contexts.User.Roles.Select(x => x.Name).Contains(role))
                {
                    await messagesService.SendResponse(x => x.RoleIsInUserAlready(contexts, role));
                    continue;
                }
                var serverRole = this._usersRolesService.GetRoleByName(role, contexts.Server);
                if (serverRole == null || !safeRolesIds.Contains(serverRole.Id))
                {
                    await messagesService.SendResponse(x => x.RoleNotFoundOrIsNotSafe(contexts, role));
                    continue;
                }
                await this._usersService.AddRoleAsync(serverRole, contexts.User, contexts.Server);
                addedRoles.Add(role);
            }
            if (addedRoles.Count > 1 && addedRoles.Count == rolesToAdd.Count)
            {
                await messagesService.SendResponse(x => x.AllRolesAddedToUser(contexts));
            }
            else
            {
                addedRoles.ForEach(async role => await messagesService.SendResponse(x => x.RoleAddedToUser(contexts, role)));
            }
        }

        public async Task DeleteRoleFromUser(IEnumerable<SafeRole> safeRoles, Contexts contexts, IReadOnlyCollection<string> rolesToRemove)
        {
            var messagesService = this._messagesServiceFactory.Create(contexts);
            var safeRolesIds = safeRoles.Select(x => x.RoleId).ToList();
            var removedRoles = new List<string>();

            foreach (var role in rolesToRemove)
            {
                if (!contexts.User.Roles.Select(x => x.Name).Contains(role))
                {
                    await messagesService.SendResponse(x => x.RoleNotFoundInUser(contexts, role));
                    continue;
                }
                var serverRole = this._usersRolesService.GetRoleByName(role, contexts.Server);
                if (serverRole == null || !safeRolesIds.Contains(serverRole.Id))
                {
                    await messagesService.SendResponse(x => x.RoleNotFoundOrIsNotSafe(contexts, role));
                    continue;
                }
                await this._usersService.RemoveRoleAsync(serverRole, contexts.User, contexts.Server);
                removedRoles.Add(role);
            }
            if (removedRoles.Count > 1 && removedRoles.Count == rolesToRemove.Count())
            {
                await messagesService.SendResponse(x => x.AllRolesRemovedFromUser(contexts));
            }
            else
            {
                removedRoles.ForEach(async role => await messagesService.SendResponse(x => x.RoleRemovedFromUser(contexts, role)));
            }
        }

        public async Task SetRolesAsSafe(Contexts contexts, IReadOnlyCollection<string> commandRoles, bool setAsSafe)
        {
            var safeRolesQuery = new GetDiscordServerSafeRolesQuery(contexts.Server.Id);
            var safeRoles = this._queryBus.Execute(safeRolesQuery).SafeRoles.ToList();
            var messagesService = this._messagesServiceFactory.Create(contexts);
            var changedRoles = new List<string>();

            foreach (var roleName in commandRoles)
            {
                var serverRole = this._usersRolesService.GetRoleByName(roleName, contexts.Server);
                if (serverRole == null)
                {
                    await messagesService.SendResponse(x => x.RoleNotFoundOrIsNotSafe(contexts, roleName));
                    continue;
                }
                var settingsWasChanged = setAsSafe
                    ? await this.TryToSetAsSafe(safeRoles, serverRole.Id, roleName, contexts)
                    : await this.TryToSetAsUnsafe(safeRoles, serverRole.Id, roleName, contexts);
                if (settingsWasChanged)
                {
                    changedRoles.Add(roleName);
                }
            }
            if (changedRoles.Count > 1 && changedRoles.Count == commandRoles.Count())
            {
                await messagesService.SendResponse(x => x.AllRolesSettingsChanged());
            }
            else
            {
                changedRoles.ForEach(async role => await messagesService.SendResponse(x => x.RoleSettingsChanged(role)));
            }
        }

        private async Task<bool> TryToSetAsSafe(IEnumerable<SafeRole> safeRoles, ulong roleId, string roleName, Contexts contexts)
        {
            if (safeRoles.Any(x => x.RoleId == roleId))
            {
                var messageService = this._messagesServiceFactory.Create(contexts);
                await messageService.SendResponse(x => x.RoleIsSafeAlready(roleName));
                return false;
            }
            await this._commandBus.ExecuteAsync(new SetRoleAsSafeCommand(roleId, contexts.Server.Id));
            return true;
        }

        private async Task<bool> TryToSetAsUnsafe(IEnumerable<SafeRole> safeRoles, ulong roleId, string roleName, Contexts contexts)
        {
            if (safeRoles.All(x => x.RoleId != roleId))
            {
                var messageService = this._messagesServiceFactory.Create(contexts);
                await messageService.SendResponse(x => x.RoleIsUnsafeAlready(roleName));
                return false;
            }
            await this._commandBus.ExecuteAsync(new SetRoleAsUnsafeCommand(roleId, contexts.Server.Id));
            return true;
        }
    }
}
