using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Users.BotCommands;
using Watchman.Discord.Areas.Users.BotCommands.Warns;
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

        public async Task<IEnumerable<WarnEvent>> GetWarns(ulong serverId, ulong userId)
        {
            var query = new GetWarnEventsQuery(serverId, userId);
            var response = await this._queryBus.ExecuteAsync(query);
            return response.WarnEvents;
        }

        public async Task GetWarns(WarnsCommand command, Contexts contexts, UserContext mentionedUser, ulong serverId)
        {
            var messageService = this._messagesServiceFactory.Create(contexts);
            if (mentionedUser == null)
            {
                await messageService.SendResponse(x => x.UserNotFound(command.User.ToString()));
            }
            else
            {
                var warnsStr = await GetWarnsToString(serverId, mentionedUser.Id);
                await messageService.SendResponse(x => x.GetUserWarns(mentionedUser.Name, warnsStr));
            }
        }

        public async Task GetAllWarns(WarnsCommand command, Contexts contexts, UserContext mentionedUser, ulong serverId)
        {
            if (!contexts.User.IsAdmin)
            {
                throw new NotAdminPermissionsException();
            }
            if (mentionedUser == null)
            {
                await this._directMessagesService.TrySendMessage(contexts.User.Id, x => x.UserNotFound(command.User.ToString()), contexts);
            }
            else
            {
                var warnsStr = await GetWarnsToString(serverId, mentionedUser.Id);
                await this._directMessagesService.TrySendMessage(contexts.User.Id, x => x.GetUserWarns(mentionedUser.Name, warnsStr), contexts);
            }
        }

        private async Task<string> GetWarnsToString(ulong serverId, ulong userId)
        {
            var warns = (await GetWarns(serverId, userId)).ToList();
            var builder = new StringBuilder();
            foreach (var warnEvent in warns)
            {
                builder.AppendLine().AppendLine().Append("Date: ").Append(warnEvent.CreatedAt.ToString())
                    .AppendLine().Append("Granted by: ").Append(warnEvent.GrantorId)
                    .AppendLine().Append("Receiver: ").Append(warnEvent.ReceiverId)
                    .AppendLine().Append("Reason: ").Append(warnEvent.Reason);
                if (serverId == 0)
                {
                    builder.AppendLine().Append("Server id: ").Append(warnEvent.ServerId.ToString());
                }
            }
            var result = builder.ToString();
            return String.IsNullOrWhiteSpace(result) ? "User has no warns" : result;
        }
    }
}
