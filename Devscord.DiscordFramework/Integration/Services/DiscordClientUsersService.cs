using System;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Rest;
using Devscord.DiscordFramework.Integration.Services.Interfaces;

namespace Devscord.DiscordFramework.Integration.Services
{
    internal class DiscordClientUsersService : IDiscordClientUsersService
    {
        private DiscordSocketRestClient _restClient => _client.Rest;
        private readonly DiscordSocketClient _client;

        public Func<SocketGuildUser, Task> UserJoined { get; set; }

        public DiscordClientUsersService(DiscordSocketClient client)
        {
            this._client = client;

            this._client.UserJoined += user => this.UserJoined(user);
        }

        public async Task<RestUser> GetUser(ulong userId)
        {
            return await this._restClient.GetUserAsync(userId);
        }

        public async Task<RestGuildUser> GetGuildUser(ulong userId, ulong guildId)
        {
            var guild = await _restClient.GetGuildAsync(guildId);
            return await guild.GetUserAsync(userId);
        }

        public async Task<IEnumerable<RestGuildUser>> GetGuildUsers(ulong guildId)
        {
            var guild = await _restClient.GetGuildAsync(guildId);
            var users = guild.GetUsersAsync();
            return await users.FlattenAsync();
        }
    }
}
