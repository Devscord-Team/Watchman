using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commands.AntiSpam;
using Devscord.DiscordFramework.Services;
using Serilog;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Protection.Models;
using Watchman.DomainModel.DiscordServer.Queries;
using Watchman.DomainModel.Messages.Queries;
using Watchman.DomainModel.Configuration.ConfigurationItems;
using Watchman.DomainModel.Configuration.Services;
using Message = Watchman.DomainModel.Messages.Message;

namespace Watchman.Discord.Areas.Protection.Strategies
{
    public interface ICheckUserSafetyService : ICyclicService, IUserSafetyChecker
    {
    }

    public class CheckUserSafetyService : ICheckUserSafetyService
    {
        private Dictionary<ulong, ServerSafeUsers> _safeUsersOnServers;
        private readonly IQueryBus _queryBus;
        private readonly IDiscordServersService _discordServersService;
        private readonly IConfigurationService _configurationService;
        private readonly IUsersService _usersService;

        public CheckUserSafetyService(IQueryBus queryBus, IDiscordServersService discordServersService, IConfigurationService configurationService, IUsersService usersService)
        {
            this._queryBus = queryBus;
            this._discordServersService = discordServersService;
            this._configurationService = configurationService;
            this._usersService = usersService;
        }

        public bool IsUserSafe(ulong userId, ulong serverId)
        {
            return this._safeUsersOnServers != null
                   && this._safeUsersOnServers.TryGetValue(serverId, out var serverUsers)
                   && serverUsers.SafeUsers.Contains(userId);
        }

        public HashSet<ulong> GetSafeUsersIds(ulong serverId)
        {
            return this._safeUsersOnServers?.GetValueOrDefault(serverId).SafeUsers ?? new HashSet<ulong>();
        }

        public async Task Refresh()
        {
            Log.Information("Reloading user safety cache....");
            await this.UpdateMessages();
            this._safeUsersOnServers.Values.ToList().ForEach(s => s.SafeUsers.ToList().ForEach(x => Log.Information("{user} is safe on {server}", x.ToString(), s.ServerId)));
            Log.Information("Cache user safety reloaded");
        }

        private async Task UpdateMessages()
        {
            var getAllMessagesQuery = new GetMessagesQuery(GetMessagesQuery.GET_ALL_SERVERS);
            var messages = this._queryBus.Execute(getAllMessagesQuery).Messages;
            await this.UpdateSafetyUsersStates(messages);
        }

        private async Task UpdateSafetyUsersStates(IEnumerable<Message> messages)
        {
            var serversWhereBotIs = await this._discordServersService.GetDiscordServersAsync().Select(x => x.Id).ToHashSetAsync();
            var servers = messages.GroupBy(x => x.Server.Id).Where(x => serversWhereBotIs.Contains(x.Key));

            this._safeUsersOnServers = servers
                .ToDictionary(x => x.Key, x =>
                {
                    var minAverageMessagesPerWeek = this._configurationService.GetConfigurationItem<MinAverageMessagesPerWeek>(x.Key).Value;
                    var minAbsoluteMessagesCount = this._configurationService.GetConfigurationItem<MinAbsoluteMessagesCountToConsiderSafeUser>(x.Key).Value;
                    var query = new GetServerTrustedRolesQuery(x.Key);
                    var trustedRolesIds = this._queryBus.Execute(query).TrustedRolesIds;
                    return new ServerSafeUsers(x, x.Key, minAverageMessagesPerWeek, minAbsoluteMessagesCount, trustedRolesIds.ToHashSet(), this._usersService, this._discordServersService);
                });
        }
    }
}
