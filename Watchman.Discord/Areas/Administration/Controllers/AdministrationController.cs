using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Architecture.Controllers;
using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Common.Models;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Administration.BotCommands;
using Watchman.Discord.Areas.Administration.Services;
using Watchman.Discord.Areas.Protection.Strategies;
using Watchman.Discord.Areas.Users.Services;
using Watchman.DomainModel.Configuration.ConfigurationItems;
using Watchman.DomainModel.Configuration.Services;
using Watchman.DomainModel.DiscordServer.Queries;

namespace Watchman.Discord.Areas.Administration.Controllers
{
    public class AdministrationController : IController
    {
        private readonly IQueryBus _queryBus;
        private readonly IUsersService _usersService;
        private readonly IDirectMessagesService _directMessagesService;
        private readonly IMessagesServiceFactory _messagesServiceFactory;
        private readonly IRolesService _rolesService;
        private readonly ITrustRolesService _trustRolesService;
        //private readonly ICheckUserSafetyService _checkUserSafetyService;
        private readonly IUsersRolesService _usersRolesService;
        private readonly IConfigurationService _configurationService;

        public AdministrationController(IQueryBus queryBus, IUsersService usersService, IDirectMessagesService directMessagesService, 
            IMessagesServiceFactory messagesServiceFactory, IRolesService rolesService, ITrustRolesService trustRolesService, 
            /*ICheckUserSafetyService checkUserSafetyService,*/ IUsersRolesService usersRolesService, IConfigurationService configurationService)
        {
            this._queryBus = queryBus;
            this._usersService = usersService;
            this._directMessagesService = directMessagesService;
            this._messagesServiceFactory = messagesServiceFactory;
            this._rolesService = rolesService;
            this._trustRolesService = trustRolesService;
            //this._checkUserSafetyService = checkUserSafetyService;
            this._usersRolesService = usersRolesService;
            this._configurationService = configurationService;
        }

        [AdminCommand]
        public Task SetRoleAsSafe(SetRoleCommand command, Contexts contexts)
        {
            if (command.Safe == command.Unsafe)
            {
                if (command.Safe)
                {
                    throw new InvalidArgumentsException();
                }
                throw new NotEnoughArgumentsException();
            }
            if (command.Roles.Count > 5 || !command.Roles.Any())
            {
                throw new InvalidArgumentsException();
            }
            return this._rolesService.SetRolesAsSafe(contexts, command.Roles, setAsSafe: command.Safe);
        }

        [AdminCommand]
        public Task SetRoleAsTrusted(TrustCommand trustCommand, Contexts contexts)
        {
            return this._trustRolesService.TrustThisRole(trustCommand.Role, contexts);
        }

        [AdminCommand]
        public Task SetRoleAsUntrusted(UntrustCommand trustCommand, Contexts contexts)
        {
            return this._trustRolesService.StopTrustingRole(trustCommand.Role, contexts);
        }

        /*
        [AdminCommand]
        public async Task GetSafeUsers(SafeUsersCommand safeUsersCommand, Contexts contexts)
        {
            var safeUsersIds = this._checkUserSafetyService.GetSafeUsersIds(contexts.Server.Id);
            var users = safeUsersIds.ToAsyncEnumerable()
                .SelectAwait(async x => await this._usersService.GetUserByIdAsync(contexts.Server, x))
                .OrderBy(x => x.JoinedServerAt());

            var values = await users.Select(user => new KeyValuePair<string, string>($"{user.Mention} -", user.JoinedServerAt()?.ToLocalTimeString() ?? string.Empty))
                .ToListAsync();

            var messagesService = this._messagesServiceFactory.Create(contexts);
            if (!values.Any())
            {
                await messagesService.SendEmbedMessage("Zaufani użytkownicy", "Serwer nie posiada zaufanych użytkowników", new Dictionary<string, string>());
                return;
            }
            var safeUsersDict = new Dictionary<string, Dictionary<string, string>>
            {
                { "Zaufani użytkownicy", new Dictionary<string, string>(values) }
            };
            await messagesService.SendEmbedMessage(
                "Zaufani użytkownicy",
                $"Lista zaufanych użytkowników na serwerze {contexts.Server.Name}",
                safeUsersDict);
        }
        */

        [AdminCommand]
        public async Task GetTrustedRoles(TrustedRolesCommand trustedRolesCommand, Contexts contexts)
        {
            var query = new GetServerTrustedRolesQuery(contexts.Server.Id);
            var trustedRoles = this._queryBus.Execute(query).TrustedRolesIds.ToList();
            var messagesService = this._messagesServiceFactory.Create(contexts);
            if (!trustedRoles.Any())
            {
                await messagesService.SendResponse(x => x.ServerDoesntHaveAnyTrustedRole());
                return;
            }
            var trustedRolesNames = trustedRoles.Select(x => this._usersRolesService.GetRole(x, contexts.Server.Id)).Where(x => x != null);
            await messagesService.SendEmbedMessage(
                "Zaufane role",
                $"Lista zaufanych roli na serwerze {contexts.Server.Name}",
                trustedRolesNames.Select(x => new KeyValuePair<string, string>("Nazwa roli:", x.Name)));
        }
    }
}
