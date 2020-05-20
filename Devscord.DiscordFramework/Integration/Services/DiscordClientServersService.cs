using System;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Middlewares.Factories;
using Discord.Rest;
using Serilog;
using Devscord.DiscordFramework.Integration.Services.Interfaces;

namespace Devscord.DiscordFramework.Integration.Services
{

    internal class DiscordClientServersService : IDiscordClientServersService
    {
        private DiscordSocketRestClient _restClient => _client.Rest;
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
            return await _restClient.GetGuildAsync(guildId);
        }

        public async Task<IEnumerable<DiscordServerContext>> GetDiscordServers()
        {
            var serverContextFactory = new DiscordServerContextFactory();
            var guilds = await _restClient.GetGuildsAsync();
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
