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

namespace Watchman.Discord.Areas.Users.Controllers
{
    public class UsersController : IController
    {
        private readonly IQueryBus queryBus;
        private readonly ICommandBus commandBus;
        private readonly MessagesServiceFactory messagesServiceFactory;
        private readonly RolesService rolesService;

        public UsersController(IQueryBus queryBus, ICommandBus commandBus, MessagesServiceFactory messagesServiceFactory, RolesService rolesService)
        {
            this.queryBus = queryBus;
            this.commandBus = commandBus;
            this.messagesServiceFactory = messagesServiceFactory;
            this.rolesService = rolesService;
        }

        [DiscordCommand("avatar")]
        public void GetAvatar(DiscordRequest request, Contexts contexts)
        {
            var messageService = messagesServiceFactory.Create(contexts);
            messageService.SendMessage(contexts.User.AvatarUrl);
        }

        [DiscordCommand("add role")] //todo
        public void AddRole(DiscordRequest request, Contexts contexts)
        {
            var commandRole = request.OriginalMessage.ToLowerInvariant().Replace("-add role ", string.Empty); //TODO use DiscordRequest properties
            var safeRoles = this.queryBus.Execute(new GetDiscordServerSafeRolesQuery()).SafeRoles;
            var messagesService = messagesServiceFactory.Create(contexts);
            rolesService.AddRoleToUser(safeRoles, messagesService, contexts, commandRole);
        }

        [DiscordCommand("remove role")] //todo
        public void RemoveRole(DiscordRequest request, Contexts contexts)
        {
            var commandRole = request.OriginalMessage.ToLowerInvariant().Replace("-remove role ", string.Empty); //TODO use DiscordRequest properties
            var safeRoles = this.queryBus.Execute(new GetDiscordServerSafeRolesQuery()).SafeRoles;
            var messagesService = messagesServiceFactory.Create(contexts);
            rolesService.DeleteRoleFromUser(safeRoles, messagesService, contexts, commandRole);
        }

        [DiscordCommand("roles")]
        public void PrintRoles(DiscordRequest request, Contexts contexts)
        {
            var messageService = messagesServiceFactory.Create(contexts);
            var safeRoles = this.queryBus.Execute(new GetDiscordServerSafeRolesQuery()).SafeRoles;
            var output = new StringBuilder();
            output.PrintManyLines("Dostępne role:", safeRoles.Select(x => x.Name).ToArray(), true);
            messageService.SendMessage(output.ToString());
        }
    }
}
