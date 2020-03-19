using System;
using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Protection.Models;
using Watchman.DomainModel.Messages;
using Watchman.DomainModel.Messages.Queries;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class UserMessagesCountService
    {
        private readonly IQueryBus _queryBus;
        private DateTime _lastUpdated;
        private List<ServerMessagesCount> _everyServersMessagesQuantity;

        public UserMessagesCountService(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        public int CountMessages(Contexts contexts)
        {
            if (ShouldReloadCache())
            {
                ReloadCache();
            }

            return GetOneUserFromOneServerCount(contexts.User.Id, contexts.Server.Id);
        }

        private bool ShouldReloadCache()
        {
            return _everyServersMessagesQuantity == null || _lastUpdated.AddHours(12) < DateTime.UtcNow;
        }

        private void ReloadCache()
        {
            _lastUpdated = DateTime.UtcNow;

            var getAllMessages = new GetMessagesQuery(0);
            var messages = _queryBus.Execute(getAllMessages).Messages.ToList();
            var groupedServerMessages = GroupMessagesIntoServers(messages);
            MakeServerMessagesCounts(groupedServerMessages);
        }

        private IEnumerable<IGrouping<ulong, Message>> GroupMessagesIntoServers(List<Message> messages)
        {
            return messages.GroupBy(x => x.Server.Id);
        }

        private void MakeServerMessagesCounts(IEnumerable<IGrouping<ulong, Message>> groups)
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