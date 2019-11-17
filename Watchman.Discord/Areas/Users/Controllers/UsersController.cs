using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Discord.Framework;
using Watchman.Discord.Framework.Architecture.Controllers;
using Watchman.Discord.Framework.Architecture.Middlewares;
using Watchman.Discord.Middlewares.Contexts;
using Watchman.Discord.Services;

namespace Watchman.Discord.Areas.Users.Controllers
{
    public class UsersController : IController
    {
        [DiscordCommand("-avatar")]
        public void GetAvatar(string message, Dictionary<string, IDiscordContext> contexts)
        {
            var user = (UserContext) contexts[nameof(UserContext)];
            var channel = (ChannelContext) contexts[nameof(ChannelContext)];

            var messageService = new MessagesService {DefaultChannelId = channel.Id};
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
            
            var guildChannel = ((SocketGuildChannel) Server.GetChannel(channelContext.Id));
            var serverRoles = Server.GetRoles(guildChannel.Guild.Id);
            var roleId = serverRoles.FirstOrDefault(x => x.Name == role?.Name)?.Id;

            if(role == null || roleId == null)
            {
                messagesService.SendMessage($"Nie znaleziono roli {commandRole} lub wybrana rola musi być dodana ręcznie przez członka administracji");
                return;
            }

            var userContext = (UserContext)contexts[nameof(UserContext)];

            if(userContext.Roles.Any(x => x == role.Name))
            {
                messagesService.SendMessage($"Użytkownik {userContext} posiada już role {commandRole}");
                return;
            }

            var userService = new UserService();
            userService.AddRole(roleId.Value, contexts);

            //todo add role id to usercontext (as object in list of roles)

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

            if (userContext.Roles.All(x => x != role.Name)) //todo change name to ID, but for this we need database
            {
                messagesService.SendMessage($"Użytkownik {userContext} nie posiada roli {commandRole}");
                return;
            }

            var guildChannel = ((SocketGuildChannel) Server.GetChannel(channelContext.Id));
            var serverRoles = Server.GetRoles(guildChannel.Guild.Id);
            var roleId = serverRoles.First(x => x.Name == role.Name).Id;

            var userService = new UserService();
            userService.RemoveRole(roleId, contexts);

            messagesService.SendMessage($"Usunięto role {commandRole} użytkownikowi {userContext}");
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
