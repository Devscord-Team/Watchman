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
using Devscord.DiscordFramework.Commons.Exceptions;
using Watchman.Discord.Areas.Users.BotCommands;
using Devscord.DiscordFramework.Services;

namespace Watchman.Discord.Areas.Users.Controllers
{
    public class UsersController : IController
    {
        private readonly IQueryBus _queryBus;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly RolesService _rolesService;
        private readonly UsersService _usersService;

        public UsersController(IQueryBus queryBus, MessagesServiceFactory messagesServiceFactory, RolesService rolesService, UsersService usersService)
        {
            this._queryBus = queryBus;
            this._messagesServiceFactory = messagesServiceFactory;
            this._rolesService = rolesService;
            this._usersService = usersService;
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

        public async Task AddRole(AddRoleCommand addRoleCommand, Contexts contexts)
        {
            var safeRoles = this._queryBus.Execute(new GetDiscordServerSafeRolesQuery(contexts.Server.Id)).SafeRoles;
            await this._rolesService.AddRoleToUser(safeRoles, contexts, addRoleCommand.Roles);
        }

        public async Task RemoveRole(RemoveRoleCommand removeRoleCommand, Contexts contexts)
        {
            var safeRoles = this._queryBus.Execute(new GetDiscordServerSafeRolesQuery(contexts.Server.Id)).SafeRoles;
            await this._rolesService.DeleteRoleFromUser(safeRoles, contexts, removeRoleCommand.Roles);
        }

        public async Task PrintRoles(RolesCommand rolesCommand, Contexts contexts)
        {
            var messageService = this._messagesServiceFactory.Create(contexts);
            var query = new GetDiscordServerSafeRolesQuery(contexts.Server.Id);
            var safeRoles = this._queryBus.Execute(query).SafeRoles.ToList();

            if (!safeRoles.Any())
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
