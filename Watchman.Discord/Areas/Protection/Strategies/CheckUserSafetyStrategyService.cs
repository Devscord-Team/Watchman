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
        private readonly DiscordServersService _discordServersService;

        public CheckUserSafetyStrategyService(IQueryBus queryBus, UsersService usersService, DiscordServersService discordServersService)
        {
            ServerSafeUsers.UsersService = usersService;
            this._queryBus = queryBus;
            this._discordServersService = discordServersService;

            this.ReloadCache().Wait();
            base.StartCyclicCaching();
        }

        public bool IsUserSafe(ulong userId, ulong serverId) => this._safeUsersOnServers.TryGetValue(serverId, out var serverUsers) && serverUsers.SafeUsers.Contains(userId);

        protected sealed override async Task ReloadCache()
        {
            Log.Information("Reloading cache....");

            this.UpdateConfiguration();
            await this.UpdateMessages();

            Log.Information("Cache reloaded");
        }

        private void UpdateConfiguration()
        {
            var query = new GetConfigurationQuery();
            var configuration = this._queryBus.Execute(query).Configuration;
            this._minAverageMessagesPerWeek = configuration.MinAverageMessagesPerWeek;
        }

        private async Task UpdateMessages()
        {
            var getAllMessagesQuery = new GetMessagesQuery(GetMessagesQuery.GET_ALL_SERVERS);
            var messages = this._queryBus.Execute(getAllMessagesQuery).Messages;
            await this.UpdateSafetyUsersStates(messages);
        }

        private async Task UpdateSafetyUsersStates(IEnumerable<Message> messages)
        {
            var serversWhereBotIs = (await this._discordServersService.GetDiscordServers()).Select(x => x.Id).ToHashSet();
            var servers = messages.GroupBy(x => x.Server.Id).Where(x => serversWhereBotIs.Contains(x.Key));

            this._safeUsersOnServers = servers.Select(x => new ServerSafeUsers(x, x.Key, this._minAverageMessagesPerWeek))
                .ToDictionary(x => x.ServerId, x => x);
        }
    }
}
