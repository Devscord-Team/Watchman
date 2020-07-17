using Devscord.DiscordFramework.Integration.Services.Interfaces;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Integration.Services
{
    internal class DiscordClientUsersService : IDiscordClientUsersService
    {
        private DiscordSocketRestClient _restClient => this._client.Rest;
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

        public async Task<bool> IsUserStillOnServer(ulong userId, ulong guildId)
        {
            return await this.GetGuildUser(userId, guildId) != null;
        }

        public async Task<RestGuildUser> GetGuildUser(ulong userId, ulong guildId)
        {
            var guild = await this._restClient.GetGuildAsync(guildId);
            return await guild.GetUserAsync(userId);
        }

        public async IAsyncEnumerable<RestGuildUser> GetGuildUsers(ulong guildId)
        {
            var guild = await this._restClient.GetGuildAsync(guildId);
            var users = guild.GetUsersAsync();
            // Discord.NET always returns all users as first and the only one element of IAsyncEnumerable
            // maybe they will make it better by returning users one by one using advantages of IAsyncEnumerable
            // at the moment - users.Count is always 1
            await foreach (var groupOfUsers in users)
            {
                foreach (var user in groupOfUsers)
                {
                    yield return user;
                }
            }
        }
    }
}
