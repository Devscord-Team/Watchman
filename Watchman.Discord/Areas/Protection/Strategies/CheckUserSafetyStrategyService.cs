using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Models;
using Serilog;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Protection.Models;
using Watchman.DomainModel.Messages.Queries;
using Watchman.DomainModel.Settings.ConfigurationItems;
using Watchman.DomainModel.Settings.Services;
using Message = Watchman.DomainModel.Messages.Message;

namespace Watchman.Discord.Areas.Protection.Strategies
{
    public class CheckUserSafetyStrategyService : ICyclicCacheGenerator, IUserSafetyChecker
    {
        public RefreshFrequent RefreshFrequent { get; } = RefreshFrequent.Daily;

        private Dictionary<ulong, ServerSafeUsers> _safeUsersOnServers;
        private readonly IQueryBus _queryBus;
        private readonly DiscordServersService _discordServersService;
        private readonly ConfigurationService _configurationService;

        public CheckUserSafetyStrategyService(IQueryBus queryBus, UsersService usersService, DiscordServersService discordServersService, ConfigurationService configurationService)
        {
            ServerSafeUsers.UsersService = usersService;
            this._queryBus = queryBus;
            this._discordServersService = discordServersService;
            this._configurationService = configurationService;
            _ = this.ReloadCache();
        }

<<<<<<< HEAD
        public bool IsUserSafe(ulong userId, ulong serverId)
        {
            return this._safeUsersOnServers.TryGetValue(serverId, out var serverUsers) && serverUsers.SafeUsers.Contains(userId);
        }
=======
        public bool IsUserSafe(ulong userId, ulong serverId)
        {
            return this._safeUsersOnServers != null 
                   && this._safeUsersOnServers.TryGetValue(serverId, out var serverUsers) 
                   && serverUsers.SafeUsers.Contains(userId);
        }
>>>>>>> master
        public async Task ReloadCache()
        {
            Log.Information("Reloading cache....");
<<<<<<< HEAD

            this.UpdateConfiguration();
            await this.UpdateMessages();

=======
            await this.UpdateMessages();
>>>>>>> master
            Log.Information("Cache reloaded");
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

            this._safeUsersOnServers = servers
                .Select(x => new ServerSafeUsers(x, x.Key, this._configurationService.GetConfigurationItem<MinAverageMessagesPerWeek>(x.Key).Value))
                .ToDictionary(x => x.ServerId, x => x);
        }
    }
}
