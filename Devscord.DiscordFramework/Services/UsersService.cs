using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Integration;
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
            var socketUser = this.GetRestUser(user, server);
            var socketRole = this.GetRole(role.Id, server);
            return socketUser.AddRoleAsync(socketRole);
        }

        public Task RemoveRole(UserRole role, UserContext user, DiscordServerContext server)
        {
            var socketUser = this.GetRestUser(user, server);
            var socketRole = this.GetRole(role.Id, server);
            return socketUser.RemoveRoleAsync(socketRole);
        }

        public IEnumerable<UserContext> GetUsers(DiscordServerContext server)
        {
            var guildUsers = Server.GetGuildUsers(server.Id).Result;

            var userContextFactory = new UserContextsFactory();
            var userContexts = guildUsers.Select(x => userContextFactory.Create(x));
            return userContexts;
        }

        public UserContext GetUserByMention(DiscordServerContext server, string mention)
        {
            var user = this.GetUsers(server)
                .FirstOrDefault(x => x.Mention.Trim("<@!&%#>".ToCharArray()) == mention.Trim("<@!&%#>".ToCharArray()));
            return user;
        }

        public UserContext GetUserById(DiscordServerContext server, ulong userId)
        {
            return this.GetUsers(server).FirstOrDefault(x => x.Id == userId);
        }

        public DateTime? GetUserJoinedDateTime(ulong userId, ulong serverId)
        {
            return Server.GetGuildUser(userId, serverId).Result?.JoinedAt?.DateTime;
        }

        private RestGuildUser GetRestUser(UserContext user, DiscordServerContext server)
        {
            return Server.GetGuildUser(user.Id, server.Id).Result;
        }

        private SocketRole GetRole(ulong roleId, DiscordServerContext server)
        {
            return Server.GetSocketRoles(server.Id).First(x => x.Id == roleId);
        }
    }
}
