using Devscord.DiscordFramework.Integration.Services.Interfaces;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Middlewares.Factories;
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

        public Func<SocketGuild, Task> BotAddedToServer { get; set; } = x => Task.CompletedTask;
        public List<DateTime> DisconnectedTimes { get; set; } = new List<DateTime>();
        public List<DateTime> ConnectedTimes { get; set; } = new List<DateTime>();

        public DiscordClientServersService(DiscordSocketClient client)
        {
            this._client = client;

            this._client.JoinedGuild += this.BotAddedToServer;
            this._client.Disconnected += this.BotDisconnected;
            this._client.Connected += this.BotConnected;
        }

        public async Task<RestGuild> GetGuild(ulong guildId)
        {
            return await this._restClient.GetGuildAsync(guildId);
        }

        public async Task<IEnumerable<DiscordServerContext>> GetDiscordServers()
        {
            var serverContextFactory = new DiscordServerContextFactory();
            var guilds = await this._restClient.GetGuildsAsync();
            var serverContexts = guilds.Select(x => serverContextFactory.Create(x));
            return serverContexts;
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
