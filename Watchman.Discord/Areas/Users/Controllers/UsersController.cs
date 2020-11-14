using System.Collections.Generic;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.DiscordServer.Queries;
using Watchman.Discord.Areas.Users.Services;
using Watchman.Discord.Areas.Users.BotCommands;

namespace Watchman.Discord.Areas.Users.Controllers
{
    public class UsersController : IController
    {
        private readonly IQueryBus _queryBus;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly RolesService _rolesService;
        private readonly ResponsesService _responsesService;

        public UsersController(IQueryBus queryBus, MessagesServiceFactory messagesServiceFactory, RolesService rolesService, ResponsesService responsesService)
        {
            this._queryBus = queryBus;
            this._messagesServiceFactory = messagesServiceFactory;
            this._rolesService = rolesService;
            this._responsesService = responsesService;
        }

        public Task GetAvatar(AvatarCommand avatarCommand, Contexts contexts)
        {
            var messageService = this._messagesServiceFactory.Create(contexts);
            if (string.IsNullOrEmpty(contexts.User.AvatarUrl))
            {
                return messageService.SendResponse(x => x.UserDoesntHaveAvatar(contexts.User));
            }
            return messageService.SendMessage(contexts.User.AvatarUrl);
        }

        public Task AddRole(AddRoleCommand addRoleCommand, Contexts contexts)
        {
            var safeRoles = this._queryBus.Execute(new GetDiscordServerSafeRolesQuery(contexts.Server.Id)).SafeRoles;
            return this._rolesService.AddRoleToUser(safeRoles, contexts, addRoleCommand.Roles);
        }

        public Task RemoveRole(RemoveRoleCommand removeRoleCommand, Contexts contexts)
        {
            var safeRoles = this._queryBus.Execute(new GetDiscordServerSafeRolesQuery(contexts.Server.Id)).SafeRoles;
            return this._rolesService.DeleteRoleFromUser(safeRoles, contexts, removeRoleCommand.Roles);
        }

        public Task PrintRoles(RolesCommand rolesCommand, Contexts contexts)
        {
            var messageService = this._messagesServiceFactory.Create(contexts);
            var query = new GetDiscordServerSafeRolesQuery(contexts.Server.Id);
            var safeRoles = this._queryBus.Execute(query).SafeRoles.ToList();
            if (!safeRoles.Any())
            {
                return messageService.SendResponse(x => x.ServerDoesntHaveAnySafeRoles());
            }
            var title = this._responsesService.GetResponse(contexts.Server.Id, x => x.AvailableSafeRoles());
            var description = this._responsesService.GetResponse(contexts.Server.Id, x => x.AvailableSafeRolesDescription());
            var serverRoles = contexts.Server.GetRoles();
            var rolesNames = safeRoles
                .Select(x => serverRoles.FirstOrDefault(serverRole => serverRole.Id == x.RoleId)?.Name)
                .Where(x => x != default);
            return messageService.SendEmbedMessage(title, description, rolesNames.Select(x => new KeyValuePair<string, string>(x, "Rola")));
        }
    }
}
