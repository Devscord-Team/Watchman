using System;
using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Protection.Models;
using Watchman.DomainModel.Messages.Queries;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class UserMessagesCountService
    {
        private readonly IQueryBus _queryBus;
        private DateTime _lastUpdated;
        private List<ServerMessagesCount> _serverMessagesQuantity;

        public UserMessagesCountService(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        public int CountMessages(Contexts contexts)
        {
            if (_serverMessagesQuantity == null || _lastUpdated.AddHours(12) < DateTime.UtcNow)
            {
                _lastUpdated = DateTime.UtcNow;

                var query = new GetMessagesQuery();
                var messages = _queryBus.Execute(query).Messages.ToList();

                var serversMessages = messages
                    .GroupBy(x => x.Server.Id)
                    .ToList();

                this._serverMessagesQuantity = serversMessages
                    .Select(x => new ServerMessagesCount(x.ToList(), x.Key))
                    .ToList();
            }

            var userCount = _serverMessagesQuantity?.FirstOrDefault(x => x.ServerId == contexts.Server.Id)?
                .UsersMessagesQuantity.FirstOrDefault(x => x.UserId == contexts.User.Id);

            return userCount?.messagesQuantity ?? 0;
        }
    }
}