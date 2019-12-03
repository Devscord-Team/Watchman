using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Services
{
    public class UserService : IService
    {
        public Task AddRole(UserRole role, UserContext user, DiscordServerContext server)
        {
            var socketUser = GetUser(user, server);
            var socketRole = GetRole(role.Id, server);
            return socketUser.AddRoleAsync(socketRole);
        }

        public Task RemoveRole(UserRole role, UserContext user, DiscordServerContext server)
        {
            var socketUser = GetUser(user, server);
            var socketRole = GetRole(role.Id, server);
            return socketUser.RemoveRoleAsync(socketRole);
        }

        public UserRole GetRoleByName(string name, DiscordServerContext server)
        {
            var role = Server.GetRoles(server.Id).FirstOrDefault(x => x.Name == name);
            if(role == null)
            {
                return default;
            }
            return new UserRole(role.Id, role.Name);
        }

        private SocketGuildUser GetUser(UserContext user, DiscordServerContext server)
        {
            return Server.GetGuildUser(user.Id, server.Id);
        }

        private SocketRole GetRole(ulong roleId, DiscordServerContext server)
        {
            return Server.GetRoles(server.Id).First(x => x.Id == roleId);
        }

        public Task WelcomeUser(ChannelContext channelContext, UserContext userContext, DiscordServerContext serverContext)
        {
            var messageService = new MessagesService()
            {
                DefaultChannelId = channelContext.Id
            };
            messageService.SendMessage($"Witaj {userContext.Mention} na serwerze {serverContext.Name}");
            return Task.CompletedTask;
        }
    }
}
