using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Protection.Strategies;
using Watchman.DomainModel.DiscordServer.Commands;
using Watchman.DomainModel.DiscordServer.Queries;

namespace Watchman.Discord.Areas.Administration.Services
{
    public class TrustRolesService
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly CheckUserSafetyService _checkUserSafetyService;
        private readonly UsersRolesService _usersRolesService;

        public TrustRolesService(IQueryBus queryBus, ICommandBus commandBus, MessagesServiceFactory messagesServiceFactory, CheckUserSafetyService checkUserSafetyService, UsersRolesService usersRolesService)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._messagesServiceFactory = messagesServiceFactory;
            this._checkUserSafetyService = checkUserSafetyService;
            this._usersRolesService = usersRolesService;
        }

        public async Task TrustThisRole(string roleName, Contexts contexts)
        {
            var query = new GetServerTrustedRolesQuery(contexts.Server.Id);
            var trustedRolesIds = this._queryBus.Execute(query).TrustedRolesIds;
            var role = this._usersRolesService.GetRoleByName(roleName, contexts.Server);
            var messagesService = this._messagesServiceFactory.Create(contexts);

            if (role == null)
            {
                throw new RoleNotFoundException(roleName);
            }
            if (trustedRolesIds.Contains(role.Id))
            {
                await messagesService.SendResponse(x => x.RoleAlreadyIsTrusted(roleName));
                return;
            }
            var command = new SetRoleAsTrustedCommand(role.Id, contexts.Server.Id);
            await this._commandBus.ExecuteAsync(command);
            await messagesService.SendResponse(x => x.RoleSetAsTrusted(roleName));
            await this._checkUserSafetyService.Refresh();
        }

        public async Task StopTrustingRole(string roleName, Contexts contexts)
        {
            var query = new GetServerTrustedRolesQuery(contexts.Server.Id);
            var trustedRolesIds = this._queryBus.Execute(query).TrustedRolesIds;
            var role = this._usersRolesService.GetRoleByName(roleName, contexts.Server);
            var messagesService = this._messagesServiceFactory.Create(contexts);

            if (role == null)
            {
                throw new RoleNotFoundException(roleName);
            }
            if (!trustedRolesIds.Contains(role.Id))
            {
                await messagesService.SendResponse(x => x.RoleAlreadyIsUntrusted(roleName));
                return;
            }
            var command = new SetRoleAsUntrustedCommand(role.Id, contexts.Server.Id);
            await this._commandBus.ExecuteAsync(command);
            await messagesService.SendResponse(x => x.RoleSetAsUntrusted(roleName));
            await this._checkUserSafetyService.Refresh();
        }

        public Task StopTrustingRole(UserRole role)
        {
            var command = new SetRoleAsUntrustedCommand(role.Id, role.ServerId);
            return this._commandBus.ExecuteAsync(command);
        }
    }
}
