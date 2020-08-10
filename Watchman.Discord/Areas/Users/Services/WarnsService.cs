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
using Watchman.Cqrs;
using Watchman.Discord.Areas.Users.BotCommands;
using Watchman.Discord.Areas.Users.BotCommands.Warns;
using Watchman.DomainModel.Messages;
using Watchman.DomainModel.Warns;
using Watchman.DomainModel.Warns.Commands;
using Watchman.DomainModel.Warns.Queries;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class WarnsService
    {
        private readonly UsersService _usersService;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly DirectMessagesService _directMessagesService;
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;

        public WarnsService(UsersService usersService, MessagesServiceFactory messagesServiceFactory, DirectMessagesService directMessagesService, ICommandBus commandBus, IQueryBus queryBus)
        {
            this._messagesServiceFactory = messagesServiceFactory;
            this._directMessagesService = directMessagesService;
            this._usersService = usersService;
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
            var warnEvents = GetWarnEvents(serverId, mentionedUser.Id);
            return WarnEventsToKeyValue(warnEvents, mentionedUser.Id);
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
                var eventKeyValuePair = new KeyValuePair<string, string>(warnEvent.CreatedAt.ToString(), warnContentBuilder.ToString());
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
