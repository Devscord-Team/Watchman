using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Middlewares.Factories;
using Discord.Rest;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Services
{
    public class UsersService
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

        public IEnumerable<UserContext> GetUsers(DiscordServerContext server)
        {
            var guildUsers = Server.GetGuildUsers(server.Id).Result;

            var userContextFactory = new UserContextsFactory();
            var userContexts = guildUsers.Select(x => userContextFactory.Create(x));
            return userContexts;
        }

        private RestGuildUser GetUser(UserContext user, DiscordServerContext server)
        {
            return Server.GetGuildUser(user.Id, server.Id).Result;
        }

        private SocketRole GetRole(ulong roleId, DiscordServerContext server)
        {
            return Server.GetSocketRoles(server.Id).First(x => x.Id == roleId);
        }
    }
}
