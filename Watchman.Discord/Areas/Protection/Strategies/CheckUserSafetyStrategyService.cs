using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Services;
using Serilog;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Protection.Models;
using Watchman.DomainModel.Messages;
using Watchman.DomainModel.Messages.Queries;
using Watchman.DomainModel.Settings.Queries;

namespace Watchman.Discord.Areas.Protection.Strategies
{
    public class CheckUserSafetyStrategyService : CyclicCacheGenerator, IUserSafetyChecker
    {
        private int _minAverageMessagesPerWeek;
        private Dictionary<ulong, ServerSafeUsers> _safeUsersOnServers;
        private readonly IQueryBus _queryBus;

        public CheckUserSafetyStrategyService(IQueryBus queryBus)
        {
            this._queryBus = queryBus;
            this.ReloadCache();
            base.StartCyclicCaching();
        }

        public bool IsUserSafe(ulong userId, ulong serverId)
        {
            return _safeUsersOnServers.TryGetValue(serverId, out var serverUsers) && serverUsers.SafeUsers.Contains(userId);
        }

        protected sealed override Task ReloadCache()
        {
            Log.Information("Reloading cache....");

            UpdateConfiguration();
            UpdateMessages();

            Log.Information("Cache reloaded");
            return Task.CompletedTask;
        }

        private void UpdateConfiguration()
        {
            var query = new GetConfigurationQuery();
            var configuration = _queryBus.Execute(query).Configuration;
            this._minAverageMessagesPerWeek = configuration.MinAverageMessagesPerWeek;
        }

        private void UpdateMessages()
        {
            var getAllMessagesQuery = new GetMessagesQuery(GetMessagesQuery.GET_ALL_SERVERS);
            var messages = _queryBus.Execute(getAllMessagesQuery).Messages;
            this.UpdateSafetyUsersStates(messages);
        }

        private void UpdateSafetyUsersStates(IEnumerable<Message> messages)
        {
            var servers = messages.GroupBy(x => x.Server.Id);
            this._safeUsersOnServers = servers.Select(x => new ServerSafeUsers(x, x.Key, _minAverageMessagesPerWeek))
                .ToDictionary(x => x.ServerId, x => x);
        }
    }
}
