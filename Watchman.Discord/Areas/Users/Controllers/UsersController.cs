<<<<<<< HEAD
﻿using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Framework.Commands.Responses;
=======
﻿using Devscord.DiscordFramework.Framework.Architecture.Controllers;
>>>>>>> master
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
<<<<<<< HEAD
=======
using Watchman.DomainModel.DiscordServer.Queries;
using Watchman.Discord.Areas.Users.Services;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Commons.Exceptions;
>>>>>>> master
using Watchman.Discord.Areas.Users.BotCommands;
using Watchman.Discord.Areas.Users.Services;
using Watchman.DomainModel.DiscordServer.Queries;

namespace Watchman.Discord.Areas.Users.Controllers
{
    public class UsersController : IController
    {
        private readonly IQueryBus _queryBus;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly RolesService _rolesService;

        public UsersController(IQueryBus queryBus, MessagesServiceFactory messagesServiceFactory, RolesService rolesService)
        {
            this._queryBus = queryBus;
            this._messagesServiceFactory = messagesServiceFactory;
            this._rolesService = rolesService;
        }

        public async Task GetAvatar(AvatarCommand avatarCommand, Contexts contexts)
        {
            var messageService = this._messagesServiceFactory.Create(contexts);
            if (string.IsNullOrEmpty(contexts.User.AvatarUrl))
            {
                await messageService.SendResponse(x => x.UserDoesntHaveAvatar(contexts.User));
                return;
            }
            await messageService.SendMessage(contexts.User.AvatarUrl);
        }

        public async Task AddRole(AddRoleCommand addRoleCommand, Contexts contexts)
        {
            if (!addRoleCommand.Roles.Any())
            {
                throw new NotEnoughArgumentsException();
            }
            var safeRoles = this._queryBus.Execute(new GetDiscordServerSafeRolesQuery(contexts.Server.Id)).SafeRoles;
<<<<<<< HEAD
            var messagesService = this._messagesServiceFactory.Create(contexts);
            this._rolesService.AddRoleToUser(safeRoles, messagesService, contexts, commandRole);
=======
            await this._rolesService.AddRoleToUser(safeRoles, contexts, addRoleCommand.Roles);
>>>>>>> master
        }

        public async Task RemoveRole(RemoveRoleCommand removeRoleCommand, Contexts contexts)
        {
            if (!removeRoleCommand.Roles.Any())
            {
                throw new NotEnoughArgumentsException();
            }
            var safeRoles = this._queryBus.Execute(new GetDiscordServerSafeRolesQuery(contexts.Server.Id)).SafeRoles;
<<<<<<< HEAD
            var messagesService = this._messagesServiceFactory.Create(contexts);
            this._rolesService.DeleteRoleFromUser(safeRoles, messagesService, contexts, commandRole);
=======
            await this._rolesService.DeleteRoleFromUser(safeRoles, contexts, removeRoleCommand.Roles);
>>>>>>> master
        }

        public async Task PrintRoles(RolesCommand rolesCommand, Contexts contexts)
        {
            var messageService = this._messagesServiceFactory.Create(contexts);
            var query = new GetDiscordServerSafeRolesQuery(contexts.Server.Id);
            var safeRoles = this._queryBus.Execute(query).SafeRoles.ToList();

            if (safeRoles.Count == 0)
            {
                await messageService.SendResponse(x => x.ServerDoesntHaveAnySafeRoles());
                return;
            }

            var output = new StringBuilder();
            output.PrintManyLines(safeRoles.Select(x => x.Name).ToArray(), contentStyleBox: false, spacesBetweenLines: false);
            await messageService.SendResponse(x => x.AvailableSafeRoles(output.ToString()));
        }
    }
}
