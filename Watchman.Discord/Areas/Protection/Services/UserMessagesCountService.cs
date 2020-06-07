using System;
using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Serilog;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Protection.Models;
using Watchman.DomainModel.Messages;
using Watchman.DomainModel.Messages.Queries;
using Watchman.DomainModel.Settings.Queries;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class UserMessagesCountService : IUserMessagesCounter
    {
        public int UserMessagesCountToBeSafe { get; private set; }

        private readonly IQueryBus _queryBus;
        private DateTime _lastUpdated;
        private List<ServerMessagesCount> _everyServersMessagesQuantity;

        public UserMessagesCountService(IQueryBus queryBus)
        {
            this._queryBus = queryBus;
        }

        public int CountUserMessages(ulong userId, ulong serverId)
        {
            if (ShouldReloadCache())
            {
                ReloadCache();
            }
            return GetOneUserFromOneServerCount(userId, serverId);
        }

        private bool ShouldReloadCache()
        {
            return _everyServersMessagesQuantity == null || _lastUpdated.AddHours(12) < DateTime.UtcNow;
        }

        private void ReloadCache()
        {
            Log.Information("Reloading cache....");

            UpdateConfiguration();
            UpdateMessages();
            _lastUpdated = DateTime.UtcNow;

            Log.Information("Cache reloaded");
        }

        private void UpdateConfiguration()
        {
            var query = new GetConfigurationQuery();
            var configuration = _queryBus.Execute(query).Configuration;
            this.UserMessagesCountToBeSafe = configuration.UserMessagesCountToBeSafe;
        }

        private void UpdateMessages()
        {
            var getAllMessagesQuery = new GetMessagesQuery(GetMessagesQuery.GET_ALL_SERVERS);
            var messages = _queryBus.Execute(getAllMessagesQuery).Messages;
            var groupedServerMessages = GroupMessagesByServers(messages);
            UpdateServerMessagesCounts(groupedServerMessages);
        }

        private IEnumerable<IGrouping<ulong, Message>> GroupMessagesByServers(IEnumerable<Message> messages)
        {
            return messages.GroupBy(x => x.Server.Id);
        }

        private void UpdateServerMessagesCounts(IEnumerable<IGrouping<ulong, Message>> groups)
        {
            this._everyServersMessagesQuantity = groups
                .Select(x => new ServerMessagesCount(x.ToList(), x.Key))
                .ToList();
        }

        private int GetOneUserFromOneServerCount(ulong userId, ulong serverId)
        {
            var userCount = _everyServersMessagesQuantity?.FirstOrDefault(x => x.ServerId == serverId)?
                .UsersMessagesQuantity.FirstOrDefault(x => x.UserId == userId);

            return userCount?.messagesQuantity ?? 0;
        }
    }
}