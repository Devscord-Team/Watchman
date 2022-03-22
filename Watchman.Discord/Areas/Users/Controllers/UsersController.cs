using System.Collections.Generic;
using Devscord.DiscordFramework.Architecture.Controllers;
using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Services;
using Watchman.Cqrs;
using Watchman.DomainModel.DiscordServer.Queries;
using Watchman.Discord.Areas.Users.Services;
using Watchman.Discord.Areas.Users.BotCommands;

namespace Watchman.Discord.Areas.Users.Controllers
{
    public class UsersController : IController
    {
        private readonly IQueryBus _queryBus;
        private readonly IMessagesServiceFactory _messagesServiceFactory;
        private readonly IRolesService _rolesService;
        private readonly IUsersService _usersService;
        private readonly IResponsesService _responsesService;

        public UsersController(IQueryBus queryBus, IMessagesServiceFactory messagesServiceFactory, IRolesService rolesService, IUsersService usersService, IResponsesService responsesService)
        {
            this._queryBus = queryBus;
            this._messagesServiceFactory = messagesServiceFactory;
            this._rolesService = rolesService;
            this._usersService = usersService;
            this._responsesService = responsesService;
        }

        public async Task GetAvatar(AvatarCommand avatarCommand, Contexts contexts)
        {
            var messageService = this._messagesServiceFactory.Create(contexts);
            var user = contexts.User;
            if (avatarCommand.User != 0)
            {
                user = await this._usersService.GetUserByIdAsync(contexts.Server, avatarCommand.User);
            }
            if (string.IsNullOrEmpty(user.AvatarUrl))
            {
                await messageService.SendResponse(x => x.UserDoesntHaveAvatar(user));
                return;
            }
            await messageService.SendMessage(user.AvatarUrl);
        }

        public Task AddRole(AddRoleCommand addRoleCommand, Contexts contexts)
        {
            if (addRoleCommand.Roles.Count > 5)
            {
                throw new InvalidArgumentsException();
            }
            var safeRoles = this._queryBus.Execute(new GetDiscordServerSafeRolesQuery(contexts.Server.Id)).SafeRoles;
            return this._rolesService.AddRoleToUser(safeRoles, contexts, addRoleCommand.Roles);
        }

        public Task RemoveRole(RemoveRoleCommand removeRoleCommand, Contexts contexts)
        {
            if (removeRoleCommand.Roles.Count > 5)
            {
                throw new InvalidArgumentsException();
            }
            var safeRoles = this._queryBus.Execute(new GetDiscordServerSafeRolesQuery(contexts.Server.Id)).SafeRoles;
            return this._rolesService.DeleteRoleFromUser(safeRoles, contexts, removeRoleCommand.Roles);
        }

        public Task PrintRoles(RolesCommand rolesCommand, Contexts contexts)
        {
            var messagesService = this._messagesServiceFactory.Create(contexts);
            var query = new GetDiscordServerSafeRolesQuery(contexts.Server.Id);
            var safeRoles = this._queryBus.Execute(query).SafeRoles.ToList();
            if (!safeRoles.Any())
            {
                return messagesService.SendResponse(x => x.ServerDoesntHaveAnySafeRoles());
            }
            var serverRoles = contexts.Server.GetRoles();
            var rolesNames = safeRoles
                .Select(x => serverRoles.FirstOrDefault(serverRole => serverRole.Id == x.RoleId)?.Name)
                .Where(x => x != default);
            return this.SendRolesAsEmbedMessage(rolesNames, messagesService, contexts.Server.Id);
        }

        private Task SendRolesAsEmbedMessage(IEnumerable<string> rolesNames, IMessagesService messagesService, ulong serverId)
        {
            var title = this._responsesService.GetResponse(serverId, x => x.AvailableSafeRoles());
            var description = this._responsesService.GetResponse(serverId, x => x.AvailableSafeRolesDescription());
            var stringBuilder = new StringBuilder("**");
            foreach (var role in rolesNames)
            {
                stringBuilder.AppendLine(role);
            }
            stringBuilder.Append("**");
            var rolesResponse = this._responsesService.GetResponse(serverId, x => x.Roles()) + ":";
            var values = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(rolesResponse, stringBuilder.ToString())
            };
            return messagesService.SendEmbedMessage(title, description, values);
        }
    }
}
