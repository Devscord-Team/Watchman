using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Cqrs;
using Watchman.DomainModel.DiscordServer;
using Watchman.DomainModel.DiscordServer.Queries;

namespace Watchman.Discord.Areas.Users.Controllers
{
    public class UsersController : IController
    {
        private readonly IQueryBus queryBus;
        private readonly ICommandBus commandBus;

        public UsersController(IQueryBus queryBus, ICommandBus commandBus)
        {
            this.queryBus = queryBus;
            this.commandBus = commandBus;
        }


        [DiscordCommand("-avatar")]
        public void GetAvatar(string message, Contexts contexts)
        {
            var messageService = new MessagesService { DefaultChannelId = contexts.Channel.Id };
            messageService.SendMessage(contexts.User.AvatarUrl);
        }

        private IEnumerable<Role> _safeRoles => this.queryBus.Execute(new GetDiscordServerSafeRolesQuery()).SafeRoles;

        //todo add system to messages management
        [DiscordCommand("-add role")]
        public void AddRole(string message, Contexts contexts)
        {
            //TODO change Dictionary to many parameters => auto dependency injection

            var commandRole = message.ToLowerInvariant().Replace("-add role ", string.Empty);
            var role = _safeRoles.FirstOrDefault(x => x.Name == commandRole);

            var messagesService = new MessagesService { DefaultChannelId = contexts.Channel.Id };

            if(role == null)
            {
                messagesService.SendMessage($"Nie znaleziono roli {commandRole} lub wybrana rola musi być dodana ręcznie przez członka administracji");
                return;
            }

            if (contexts.User.Roles.Any(x => x.Name == role.Name))
            {
                messagesService.SendMessage($"Użytkownik {contexts.User.Name} posiada już role {commandRole}");
                return;
            }
            var userService = new UserService();
            var serverRole = userService.GetRoleByName(commandRole, contexts.Server);
            userService.AddRole(serverRole, contexts.User, contexts.Server).Wait();
            messagesService.SendMessage($"Dodano role {commandRole} użytkownikowi {contexts.User.Name}");
        }

        [DiscordCommand("-remove role")]
        public void RemoveRole(string message, Contexts contexts)
        {
            var commandRole = message.ToLowerInvariant().Replace("-remove role ", string.Empty);
            var role = _safeRoles.FirstOrDefault(x => x.Name == commandRole);

            var messagesService = new MessagesService { DefaultChannelId = contexts.Channel.Id };

            if (role == null)
            {
                messagesService.SendMessage($"Nie znaleziono roli {commandRole} lub wybrana rola musi być usunięta ręcznie przez członka administracji");
                return;
            }

            if (contexts.User.Roles.All(x => x.Name != role.Name)) 
            {
                messagesService.SendMessage($"Użytkownik {contexts.User} nie posiada roli {commandRole}");
                return;
            }

            var userService = new UserService();
            var serverRole = userService.GetRoleByName(commandRole, contexts.Server);
            userService.RemoveRole(serverRole, contexts.User, contexts.Server).Wait();
            messagesService.SendMessage($"Usunięto role {commandRole} użytkownikowi {contexts.User.Name}");
        }

        [DiscordCommand("-role list")]
        [DiscordCommand("-roles")]
        public void PrintRoles(string message, Contexts contexts)
        {
            var messageService = new MessagesService { DefaultChannelId = contexts.Channel.Id };
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Dostępne role:");
            stringBuilder.AppendLine("```");
            _safeRoles.ToList().ForEach(x => stringBuilder.AppendLine(x.Name));
            stringBuilder.Append("```");
            messageService.SendMessage(stringBuilder.ToString());
        }
    }

    
}
