using System.Collections.Generic;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using System.Linq;
using System.Text;
using Devscord.DiscordFramework.Commons.Extensions;
using Watchman.Cqrs;
using Watchman.DomainModel.DiscordServer.Queries;
using Watchman.Discord.Areas.Users.Services;
using Watchman.DomainModel.DiscordServer;
using Devscord.DiscordFramework.Framework.Commands.Responses;

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

        [DiscordCommand("avatar")]
        public void GetAvatar(DiscordRequest request, Contexts contexts)
        {
            var messageService = _messagesServiceFactory.Create(contexts);
            if (string.IsNullOrEmpty(contexts.User.AvatarUrl))
            {
                messageService.SendResponse(x => x.UserDoesntHaveAvatar(contexts.User), contexts);
                return;
            }

            messageService.SendMessage(contexts.User.AvatarUrl);
        }

        [DiscordCommand("add role")] //todo
        public void AddRole(DiscordRequest request, Contexts contexts)
        {
            var commandRole = request.OriginalMessage.ToLowerInvariant().Replace("-add role ", string.Empty); //TODO use DiscordRequest properties
            var safeRoles = this._queryBus.Execute(new GetDiscordServerSafeRolesQuery(contexts.Server.Id)).SafeRoles;
            var messagesService = _messagesServiceFactory.Create(contexts);
            _rolesService.AddRoleToUser(safeRoles, messagesService, contexts, commandRole);
        }

        [DiscordCommand("remove role")] //todo
        public void RemoveRole(DiscordRequest request, Contexts contexts)
        {
            var commandRole = request.OriginalMessage.ToLowerInvariant().Replace("-remove role ", string.Empty); //TODO use DiscordRequest properties
            var safeRoles = this._queryBus.Execute(new GetDiscordServerSafeRolesQuery(contexts.Server.Id)).SafeRoles;
            var messagesService = _messagesServiceFactory.Create(contexts);
            _rolesService.DeleteRoleFromUser(safeRoles, messagesService, contexts, commandRole);
        }

        [DiscordCommand("roles")]
        public void PrintRoles(DiscordRequest request, Contexts contexts)
        {
            var messageService = _messagesServiceFactory.Create(contexts);
            var safeRoles = this._queryBus.Execute(new GetDiscordServerSafeRolesQuery(contexts.Server.Id)).SafeRoles;
            var output = new StringBuilder();
            output.PrintManyLines("Dostępne role:", safeRoles.Select(x => x.Name).ToArray(), true);
            messageService.SendMessage(output.ToString());
        }

#if DEBUG
        [DiscordCommand("admin")]
        public void AddOrRemoveAdmin(DiscordRequest request, Contexts contexts)
        {
            var roles = new List<Role> { new Role("admin", contexts.Server.Id) };
            var messagesService = _messagesServiceFactory.Create(contexts);
            if (contexts.User.IsAdmin)
            {
                _rolesService.DeleteRoleFromUser(roles, messagesService, contexts, "admin");
            }
            else
            {
                _rolesService.AddRoleToUser(roles, messagesService, contexts, "admin");
            }
        }
#endif
    }
}
