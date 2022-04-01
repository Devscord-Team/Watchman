using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Commands.Responses;
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
using Watchman.Cqrs;
using Watchman.DomainModel.Warns;
using Watchman.DomainModel.Warns.Commands;
using Watchman.DomainModel.Warns.Queries;
using Watchman.Discord.Areas.Warns.BotCommands;

namespace Watchman.Discord.Areas.Warns.Services
{
    public interface IWarnsService
    {
        Task AddWarnToUser(AddWarnCommand command, Contexts contexts, UserContext targetUser);
        IEnumerable<KeyValuePair<string, string>> GetWarns(UserContext mentionedUser, ulong serverId);
        IEnumerable<WarnEvent> GetWarnEvents(ulong serverId, ulong userId);
    }

    public class WarnsService : IWarnsService
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;

        public WarnsService(ICommandBus commandBus, IQueryBus queryBus)
        {
            this._commandBus = commandBus;
            this._queryBus = queryBus;
        }

        public Task AddWarnToUser(AddWarnCommand command, Contexts contexts, UserContext targetUser)
        {
            var addWarnEventCommand = new AddWarnEventCommand(contexts.User.Id, targetUser.Id, command.Reason, contexts.Server.Id);
            return this._commandBus.ExecuteAsync(addWarnEventCommand);
        }

        public IEnumerable<KeyValuePair<string, string>> GetWarns(UserContext mentionedUser, ulong serverId)
        {
            var warnEvents = this.GetWarnEvents(serverId, mentionedUser.Id);
            return this.WarnEventsToKeyValue(warnEvents, mentionedUser.Id);
        }

        public IEnumerable<WarnEvent> GetWarnEvents(ulong serverId, ulong userId)
        {
            var query = new GetWarnEventsQuery(serverId, userId);
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
