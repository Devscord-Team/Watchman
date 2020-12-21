using System;
using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Services;
using Serilog;

namespace Devscord.DiscordFramework.Framework.Commands.Responses
{
    public class ResponsesCachingService
    {
        public Func<ulong, IEnumerable<Response>> GetResponsesFunc { get; set; } = x => throw new NotImplementedException();

        private readonly DiscordServersService _discordServersService;
        private static readonly Dictionary<ulong, IEnumerable<Response>> _serversResponses = new Dictionary<ulong, IEnumerable<Response>>();

        public ResponsesCachingService(DiscordServersService discordServersService)
        {
            this._discordServersService = discordServersService;
        }

        public IEnumerable<Response> GetResponses(ulong serverId)
        {
            if (!_serversResponses.ContainsKey(serverId))
            {
                this.InitResponsesCache();
            }
            return _serversResponses[serverId];
        }

        public void UpdateServerResponses(ulong serverId)
        {
            var responses = this.GetResponsesFunc.Invoke(serverId);
            if (_serversResponses.ContainsKey(serverId))
            {
                _serversResponses[serverId] = responses;
                return;
            }
            _serversResponses.Add(serverId, responses);
        }

        private void InitResponsesCache()
        {
            Log.Information("Refreshing responses cache...");
            foreach (var server in this._discordServersService.GetDiscordServersAsync().ToEnumerable())
            {
                var responses = this.GetResponsesFunc.Invoke(server.Id);
                _serversResponses.Add(server.Id, responses);
            }
            Log.Information("Responses cache refreshed");
        }
    }
}
