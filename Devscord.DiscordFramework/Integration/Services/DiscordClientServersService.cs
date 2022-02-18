using Devscord.DiscordFramework.Integration.Services.Interfaces;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Middlewares.Factories;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Integration.Services
{
    internal class DiscordClientServersService : IDiscordClientServersService
    {
        private DiscordSocketRestClient _restClient => this._client.Rest;
        private readonly DiscordSocketClient _client;
        private readonly IDiscordServerContextFactory _discordServerContextFactory;

        public Func<IGuild, Task> BotAddedToServer { get; set; } = x => Task.CompletedTask;
        public List<DateTime> DisconnectedTimes { get; set; } = new List<DateTime>();
        public List<DateTime> ConnectedTimes { get; set; } = new List<DateTime>();

        public DiscordClientServersService(DiscordSocketClient client, IDiscordServerContextFactory discordServerContextFactory)
        {
            this._client = client;
            this._discordServerContextFactory = discordServerContextFactory;

            this._client.JoinedGuild += this.BotAddedToServer;
            this._client.Disconnected += this.BotDisconnected;
            this._client.Connected += this.BotConnected;
        }

        public async Task<IGuild> GetGuild(ulong guildId)
        {
            return await this._restClient.GetGuildAsync(guildId);
        }

        public IAsyncEnumerable<DiscordServerContext> GetDiscordServersAsync()
        {
            return this._client.Guilds.ToAsyncEnumerable().Select(guild => this._discordServerContextFactory.Create(guild));
        }

        public Task<DiscordServerContext> GetDiscordServerAsync(ulong serverId)
        {
            var guild = this._client.GetGuild(serverId);
            return Task.FromResult(this._discordServerContextFactory.Create(guild));
        }

        public async Task<IEnumerable<string>> GetExistingInviteLinks(ulong serverId)
        {
            var guild = this._client.GetGuild(serverId);
            var invites = await guild.GetInvitesAsync();
            return invites.Select(x => x.Url);
        }

        private Task BotDisconnected(Exception exception)
        {
            Log.Warning(exception, "Bot disconnected!");
            this.DisconnectedTimes.Add(DateTime.Now);
            return Task.CompletedTask;
        }

        private Task BotConnected()
        {
            this.ConnectedTimes.Add(DateTime.Now);
            return Task.CompletedTask;
        }
    }
}
