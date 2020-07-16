using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Middlewares.Factories;
using Discord.Rest;
using Discord.WebSocket;
using Serilog;

namespace Devscord.DiscordFramework.Services
{
    public class UsersService
    {
        private readonly Regex exMention = new Regex(@"\d+", RegexOptions.Compiled);

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
            Log.Information("Found {users} on server {server}", guildUsers.Count(), server.Name);
            var userContextFactory = new UserContextsFactory();
            var userContexts = guildUsers.Select(x => userContextFactory.Create(x));
            return userContexts;
        }

        public UserContext GetUserByMention(DiscordServerContext server, string mention)
        {
            var match = exMention.Match(mention);
            if(!match.Success)
            {
                Log.Warning("Mention {mention} has not user ID", mention);
                return null;
            }
            var id = ulong.Parse(match.Value);
            var user = this.GetUserById(server, id);
            return user;
        }

        public UserContext GetUserById(DiscordServerContext server, ulong userId)
        {
            var user = this.GetUsers(server).FirstOrDefault(x => x.Id == userId);
            if(user == null)
            {
                Log.Warning("Cannot get user by id {userId}", userId);
            }
            return user;
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
