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

namespace Watchman.Discord.Areas.Users.Controllers
{
    public class UsersController : IController
    {
        [DiscordCommand("-avatar")]
        public void GetAvatar(string message, Dictionary<string, IDiscordContext> contexts)
        {
            var user = (UserContext) contexts[nameof(UserContext)];
            var channel = (ChannelContext) contexts[nameof(ChannelContext)];

            var messageService = new MessagesService { DefaultChannelId = channel.Id };
            messageService.SendMessage(user.AvatarUrl);
        }

        //todo database
        private readonly List<Role> _safeRoles = new List<Role>
        {
            new Role("csharp"),
            new Role("java"),
            new Role("cpp"),
            new Role("tester"),
            new Role("javascript"),
            new Role("python"),
            new Role("php"),
            new Role("functional master"),
            new Role("rust"),
            new Role("go"),
            new Role("ruby"),
            new Role("newbie"),
        };

        //todo add system to messages management
        [DiscordCommand("-add role")]
        public void AddRole(string message, Dictionary<string, IDiscordContext> contexts)
        {
            //TODO change Dictionary to many parameters => auto dependency injection

            var commandRole = message.ToLowerInvariant().Replace("-add role ", string.Empty);
            var role = _safeRoles.FirstOrDefault(x => x.Name == commandRole);

            var channelContext = (ChannelContext)contexts[nameof(ChannelContext)];
            var messagesService = new MessagesService { DefaultChannelId = channelContext.Id };

            if(role == null)
            {
                messagesService.SendMessage($"Nie znaleziono roli {commandRole} lub wybrana rola musi być dodana ręcznie przez członka administracji");
                return;
            }

            var userContext = (UserContext)contexts[nameof(UserContext)];
            if (userContext.Roles.Any(x => x.Name == role.Name))
            {
                messagesService.SendMessage($"Użytkownik {userContext} posiada już role {commandRole}");
                return;
            }

            var serverContext = (DiscordServerContext)contexts[nameof(DiscordServerContext)];
            var userService = new UserService();
            var serverRole = userService.GetRoleByName(commandRole, serverContext);
            userService.AddRole(serverRole, userContext, serverContext);

            messagesService.SendMessage($"Dodano role {commandRole} użytkownikowi {userContext}");
        }

        [DiscordCommand("-remove role")]
        public void RemoveRole(string message, Dictionary<string, IDiscordContext> contexts)
        {
            var commandRole = message.ToLowerInvariant().Replace("-remove role ", string.Empty);
            var role = _safeRoles.FirstOrDefault(x => x.Name == commandRole);

            var channelContext = (ChannelContext)contexts[nameof(ChannelContext)];
            var messagesService = new MessagesService { DefaultChannelId = channelContext.Id };

            if (role == null)
            {
                messagesService.SendMessage($"Nie znaleziono roli {commandRole} lub wybrana rola musi być usunięta ręcznie przez członka administracji");
                return;
            }

            var userContext = (UserContext)contexts[nameof(UserContext)];
            if (userContext.Roles.All(x => x.Name != role.Name)) 
            {
                messagesService.SendMessage($"Użytkownik {userContext} nie posiada roli {commandRole}");
                return;
            }

            var serverContext = (DiscordServerContext)contexts[nameof(DiscordServerContext)];
            var userService = new UserService();
            var serverRole = userService.GetRoleByName(commandRole, serverContext);
            userService.RemoveRole(serverRole, userContext, serverContext);

            messagesService.SendMessage($"Usunięto role {commandRole} użytkownikowi {userContext}");
        }

        [DiscordCommand("-role list")]
        [DiscordCommand("-roles")]
        public void PrintRoles(string message, Dictionary<string, IDiscordContext> contexts)
        {
            var channelContext = (ChannelContext)contexts[nameof(ChannelContext)];
            var messageService = new MessagesService { DefaultChannelId = channelContext.Id };

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Dostępne role:");
            stringBuilder.AppendLine("```");
            _safeRoles.ForEach(x => stringBuilder.AppendLine(x.Name));
            stringBuilder.Append("```");

            messageService.SendMessage(stringBuilder.ToString());
        }
    }

    public class Role
    {
        public string Name { get; private set; }

        public Role(string name)
        {
            this.Name = name;
        }
    }
}
