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
        private readonly UserContextsFactory _userContextsFactory;
        private readonly Regex _exMention = new Regex(@"\d+", RegexOptions.Compiled);

        public UsersService()
        {
            this._userContextsFactory = new UserContextsFactory();
        }

        public async Task AddRoleAsync(UserRole role, UserContext user, DiscordServerContext server)
        {
            var socketUser = await this.GetRestUser(user, server);
            var socketRole = this.GetRole(role.Id, server);
            await socketUser.AddRoleAsync(socketRole);
        }

        public async Task RemoveRoleAsync(UserRole role, UserContext user, DiscordServerContext server)
        {
            var socketUser = await this.GetRestUser(user, server);
            var socketRole = this.GetRole(role.Id, server);
            await socketUser.RemoveRoleAsync(socketRole);
        }

        public async IAsyncEnumerable<UserContext> GetUsersAsync(DiscordServerContext server)
        {
            var guildUsers = Server.GetGuildUsers(server.Id);
            await foreach (var user in guildUsers)
            {
                yield return this._userContextsFactory.Create(user);
            }
        }

        public async Task<UserContext> GetUserByMentionAsync(DiscordServerContext server, string mention)
        {
            var match = this._exMention.Match(mention);
            if(!match.Success)
            {
                Log.Warning("Mention {mention} has not user ID", mention);
                return null;
            }
            var id = ulong.Parse(match.Value);
            var user = await this.GetUserByIdAsync(server, id);
            return user;
        }

        public Task<bool> IsUserStillOnServerAsync(DiscordServerContext server, ulong userId)
        {
            return Server.IsUserStillOnServer(userId, server.Id);
        }

        public async Task<UserContext> GetUserByIdAsync(DiscordServerContext server, ulong userId)
        {
            var user = await Server.GetGuildUser(userId, server.Id);
            if(user == null)
            {
                Log.Warning("Cannot get user by id {userId}", userId);
                return null;
            }
            return this._userContextsFactory.Create(user);
        }

        public DateTime? GetUserJoinedDateTime(ulong userId, ulong serverId)
        {
            return Server.GetGuildUser(userId, serverId).Result?.JoinedAt?.DateTime;
        }

        private async Task<RestGuildUser> GetRestUser(UserContext user, DiscordServerContext server)
        {
            return await Server.GetGuildUser(user.Id, server.Id);
        }

        private SocketRole GetRole(ulong roleId, DiscordServerContext server)
        {
            return Server.GetSocketRoles(server.Id).First(x => x.Id == roleId);
        }
    }
}
