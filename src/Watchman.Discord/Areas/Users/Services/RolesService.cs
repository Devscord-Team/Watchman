﻿using Devscord.DiscordFramework.Framework.Commands.Responses;
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

        public async Task AddRoleToUser(IEnumerable<Role> safeRoles, Contexts contexts, IEnumerable<string> rolesToAdd)
        {
            var messagesService = this._messagesServiceFactory.Create(contexts);
            var userRoleNames = contexts.User.Roles.Select(x => x.Name).ToList();
            var safeRoleNames = safeRoles.Select(x => x.Name).ToList();
            var addedRoles = new List<string>();

            foreach (var role in rolesToAdd)
            {
                if (userRoleNames.Contains(role))
                {
                    await messagesService.SendResponse(x => x.RoleIsInUserAlready(contexts, role));
                    continue;
                }
                var serverRole = this._usersRolesService.GetRoleByName(role, contexts.Server);
                if (serverRole == null || !safeRoleNames.Contains(role))
                {
                    await messagesService.SendResponse(x => x.RoleNotFoundOrIsNotSafe(contexts, role));
                    continue;
                }
                await this._usersService.AddRoleAsync(serverRole, contexts.User, contexts.Server);
                addedRoles.Add(role);
            }
            if (addedRoles.Count > 1 && addedRoles.Count == rolesToAdd.Count())
            {
                await messagesService.SendResponse(x => x.AllRolesAddedToUser(contexts));
            }
            else
            {
                addedRoles.ForEach(async role => await messagesService.SendResponse(x => x.RoleAddedToUser(contexts, role)));
            }
        }

        public async Task DeleteRoleFromUser(IEnumerable<Role> safeRoles, Contexts contexts, IEnumerable<string> rolesToRemove)
        {
            var messagesService = this._messagesServiceFactory.Create(contexts);
            var userRoleNames = contexts.User.Roles.Select(x => x.Name).ToList();
            var safeRoleNames = safeRoles.Select(x => x.Name).ToList();
            var removedRoles = new List<string>();

            foreach (var role in rolesToRemove)
            {
                if (!userRoleNames.Contains(role))
                {
                    await messagesService.SendResponse(x => x.RoleNotFoundInUser(contexts, role));
                    continue;
                }
                var serverRole = this._usersRolesService.GetRoleByName(role, contexts.Server);
                if (serverRole == null || !safeRoleNames.Contains(role))
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

        public async Task SetRolesAsSafe(Contexts contexts, IEnumerable<string> commandRoles, bool setAsSafe)
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
                    ? await this.TryToSetAsSafe(safeRoles, roleName, contexts)
                    : await this.TryToSetAsUnsafe(safeRoles, roleName, contexts);
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
