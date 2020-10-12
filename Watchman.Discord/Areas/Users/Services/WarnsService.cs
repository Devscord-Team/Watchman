using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using MongoDB.Driver;
using MongoDB.Driver.Core.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Common.Models;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Protection.Services;
using Watchman.Discord.Areas.Users.BotCommands;
using Watchman.Discord.Areas.Users.BotCommands.Warns;
using Watchman.DomainModel.Messages;
using Watchman.DomainModel.Warns;
using Watchman.DomainModel.Warns.Commands;
using Watchman.DomainModel.Warns.Queries;

namespace Watchman.Discord.Areas.Users.Services
{
    public class WarnsService : IWarnsService
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;
        private readonly PunishmentsCachingService _punishmentsCachingService;

        public WarnsService(ICommandBus commandBus, IQueryBus queryBus, PunishmentsCachingService punishmentsCachingService)
        {
            this._commandBus = commandBus;
            this._queryBus = queryBus;
            this._punishmentsCachingService = punishmentsCachingService;
        }

        public Task AddWarnToUser(ulong grantorId, ulong receiverId, string reason, ulong serverId)
        {
            var addWarnEventCommand = new AddWarnEventCommand(grantorId, receiverId, reason, serverId);
            this._punishmentsCachingService.AddWarnLocal(grantorId, receiverId, reason, serverId);
            return this._commandBus.ExecuteAsync(addWarnEventCommand);
        }

        public Task RemoveUserWarns(ulong receiverId, ulong serverId)
        {
            var removeWarnsCommand = new RemoveWarnEventsCommand(grantorId: null, receiverId, serverId, from: new DateTime());
            this._punishmentsCachingService.RemoveWarnsLocal(serverId, userId: receiverId, from: new DateTime());
            return this._commandBus.ExecuteAsync(removeWarnsCommand);
        }

        public IEnumerable<KeyValuePair<string, string>> GetWarns(UserContext mentionedUser, ulong serverId)
        {
            var warnEvents = this.GetWarnEvents(serverId, mentionedUser.Id);
            return this.WarnEventsToKeyValue(warnEvents, mentionedUser.Id);
        }

        public int GetWarnsCount(ulong userId, ulong serverId, DateTime from)
        {
            return this.GetWarnEvents(serverId, userId, from).Count();
        }

        public IEnumerable<WarnEvent> GetWarnEvents(ulong serverId, ulong receiverId, DateTime from =new DateTime())
        {
            var query = new GetWarnEventsQuery(serverId, receiverId, new TimeRange(from, DateTime.Now));
            var response = this._queryBus.Execute(query);
            return response.WarnEvents;
        }

        private IEnumerable<KeyValuePair<string, string>> WarnEventsToKeyValue(IEnumerable<WarnEvent> warns, ulong mentionedUserId)
        {
            var warnEventPairs = new List<KeyValuePair<string, string>>();
            foreach (var warnEvent in warns)
            {
                var warnContentBuilder = new StringBuilder();
                warnContentBuilder.Append("Nadane przez: ").Append(warnEvent.GrantorId.GetUserMention())
                    .AppendLine().Append("Odbiorca: ").Append(warnEvent.ReceiverId.GetUserMention())
                    .AppendLine().Append("Powód: ").Append(warnEvent.Reason);
                var eventKeyValuePair = new KeyValuePair<string, string>(warnEvent.CreatedAt.ToLocalTimeString(), warnContentBuilder.ToString());
                warnEventPairs.Add(eventKeyValuePair);
            }
            if (!warnEventPairs.Any())
            {
                warnEventPairs.Add(new KeyValuePair<string, string>("Brak zawartości", $"Użytkownik {mentionedUserId.GetUserMention()} nie ma żadnych ostrzeżeń."));
            }
            return warnEventPairs;
        }
    }
}
