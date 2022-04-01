using Devscord.DiscordFramework.Integration.Services.Interfaces;
using Discord.Rest;
using Discord.WebSocket;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using System.Diagnostics.CodeAnalysis;

namespace Devscord.DiscordFramework.Integration.Services
{
    [ExcludeFromCodeCoverage]
    internal class DiscordClientUsersService : IDiscordClientUsersService
    {
        private DiscordSocketRestClient _restClient => this._client.Rest;
        private readonly DiscordSocketClient _client;

        public Func<IGuildUser, Task> UserJoined { get; set; }

        public DiscordClientUsersService(DiscordSocketClient client)
        {
            this._client = client;
            this._client.UserJoined += user => this.UserJoined(user);
        }

        public async Task<IUser> GetUser(ulong userId)
        {
            var user = await this._restClient.GetUserAsync(userId);
            if (user == null)
            {
                Log.Information("Couldn't get user {userId}", userId);
            }
            else
            {
                Log.Information("Got {userName}", user.Username);
            }
            return user;
        }

        public async Task<bool> IsUserStillOnServer(ulong userId, ulong guildId)
        {
            return await this.GetGuildUser(userId, guildId) != null;
        }

        public async Task<IGuildUser> GetGuildUser(ulong userId, ulong guildId)
        {
            var guild = await this._restClient.GetGuildAsync(guildId);
            var user = await guild.GetUserAsync(userId);
            if (user == null)
            {
                Log.Information("Couldn't get user {userId} on server {guildId}", userId, guildId);
            }
            else
            {
                Log.Information("Got {userName}", user.Username);
            }
            return user;
        }

        public IEnumerable<IGuildUser> GetGuildUsers(ulong guildId)
        {
            var guild = this._client.GetGuild(guildId);
            var users = guild.Users;
            foreach (var user in users)
            {
                yield return user;
            }
        }

        public IUser GetBotUser()
        {
            return this._restClient.CurrentUser;
        }
    }
}
