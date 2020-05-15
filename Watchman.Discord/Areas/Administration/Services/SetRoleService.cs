using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.DiscordServer.Commands;
using Watchman.DomainModel.DiscordServer.Queries;

namespace Watchman.Discord.Areas.Administration.Services
{
    public class SetRoleService
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly UsersRolesService _usersRolesService;
        public SetRoleService(MessagesServiceFactory messagesServiceFactory, IQueryBus queryBus, ICommandBus commandBus, UsersRolesService usersRolesService)
        {
            this._messagesServiceFactory = messagesServiceFactory;
            this._queryBus = queryBus;
            this._usersRolesService = usersRolesService;
            this._commandBus = commandBus;
        }

        public async Task SetRolesAsSafe(Contexts contexts, IEnumerable<string> commandRoles, bool setAsSafe)
        {
            var safeRolesQuery = new GetDiscordServerSafeRolesQuery(contexts.Server.Id);
            var safeRoles = this._queryBus.Execute(safeRolesQuery).SafeRoles;
            var messageService = _messagesServiceFactory.Create(contexts);
            foreach (var roleName in commandRoles)
            {
                var serverRole = _usersRolesService.GetRoleByName(roleName, contexts.Server);

                if (serverRole == null)
                {
                    await messageService.SendResponse(x => x.RoleNotFoundOrIsNotSafe(contexts, roleName), contexts);
                    continue;
                }

                if (setAsSafe && !safeRoles.Select(x => x.Name).Where(x => x == roleName).Select(x => x).Any())
                {
                    await messageService.SendResponse(x => x.RoleIsSafeAlready(roleName), contexts);
                    continue;
                }
                else if (!setAsSafe && !safeRoles.Select(x => x.Name).Where(x => x == roleName).Select(x => x).Any())
                {
                    await messageService.SendResponse(x => x.RoleIsUnsafeAlready(roleName), contexts);
                    continue;
                }

                if (setAsSafe)
                {
                    await _commandBus.ExecuteAsync(new SetRoleAsSafeCommand(roleName, contexts.Server.Id));
                }
                else
                {
                    await _commandBus.ExecuteAsync(new SetRoleAsUnsafeCommand(roleName, contexts.Server.Id));
                }
            }

        }
    }
}
