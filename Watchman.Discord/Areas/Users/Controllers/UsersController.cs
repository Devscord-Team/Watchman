using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Discord.Framework;
using Watchman.Discord.Framework.Architecture.Controllers;

namespace Watchman.Discord.Areas.Users.Controllers
{
    public class UsersController : IController
    {
        [DiscordCommand("-avatar")]
        public void GetAvatar(SocketMessage message)
        {
            var avatar = message.Author.GetAvatarUrl(ImageFormat.Png, 2048);
            //todo message
            message.Channel.SendMessageAsync(avatar);
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
        public void AddRole(SocketMessage message)
        {
            var commandRole = message.Content.Replace("-add role ", string.Empty);

            var role = _safeRoles.FirstOrDefault(x => x.Name == commandRole);
            if(role == null)
            {
                //todo message
                message.Channel.SendMessageAsync($"Nie znaleziono roli {commandRole} lub wybrana rola musi być dodana ręcznie przez członka administracji");
                return;
            }
            var user = (SocketGuildUser) message.Author;
            if(user.Roles.Any(x => x.Name == role.Name)) //todo change name to ID, but for that we need database
            {
                //todo message
                message.Channel.SendMessageAsync($"Użytkownik {user.ToString()} posiada już role {commandRole}");
                return;
            }
            var serverRole = Server.GetRoles(user.Guild.Id).First(x => x.Name == role.Name); // todo change name to id

            user.AddRoleAsync(serverRole).Wait();
            //todo message
            message.Channel.SendMessageAsync($"Dodano role {commandRole} użytkownikowi {user.ToString()}");
        }

        [DiscordCommand("-remove role")]
        public void RemoveRole(SocketMessage message)
        {
            var commandRole = message.Content.Replace("-remove role ", string.Empty);
            var role = _safeRoles.FirstOrDefault(x => x.Name == commandRole);
            if (role == null)
            {
                //todo message
                message.Channel.SendMessageAsync($"Nie znaleziono roli {commandRole} lub wybrana rola musi być usunięta ręcznie przez członka administracji");
                return;
            }
            var user = (SocketGuildUser)message.Author;
            if (!user.Roles.Any(x => x.Name == role.Name)) //todo change name to ID, but for that we need database
            {
                //todo message
                message.Channel.SendMessageAsync($"Użytkownik {user.ToString()} nie posiada roli {commandRole}");
                return;
            }
            var serverRole = Server.GetRoles(user.Guild.Id).First(x => x.Name == role.Name); // todo change name to id

            user.RemoveRoleAsync(serverRole).Wait();
            //todo message
            message.Channel.SendMessageAsync($"Usunięto role {commandRole} użytkownikowi {user.ToString()}");
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
