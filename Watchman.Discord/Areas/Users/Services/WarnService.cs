using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Users.BotCommands;
using Watchman.DomainModel.Messages;
using Watchman.DomainModel.Users;
using Watchman.DomainModel.Users.Commands.Handlers;
using Watchman.DomainModel.Users.Queries;
using Watchman.DomainModel.Warnings;
using Watchman.DomainModel.Warnings.Commands;
using Watchman.DomainModel.Warnings.Queries;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class WarnService
    {
        private readonly UsersService _usersService;
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;

        public WarnService(UsersService usersService, ICommandBus commandBus, IQueryBus queryBus)
        {
            this._usersService = usersService;
            this._commandBus = commandBus;
            this._queryBus = queryBus;
        }

        public Task AddWarnToUser(AddWarnCommand command, Contexts contexts, UserContext targetUser)
        {
            var dbCommand = new AddWarnEventCommand(contexts.User.Id, targetUser.Id, command.Reason, contexts.Server.Id);
            return _commandBus.ExecuteAsync(dbCommand);
        }

        public async Task<IEnumerable<WarnEvent>> GetWarns(ulong serverId, ulong userId)
        {
            var query = new GetWarnEventsQuery(serverId, userId);
            var response = await _queryBus.ExecuteAsync(query);
            return response.WarnEvents;
        }

        public async Task<string> GetWarnsToString(ulong serverId, ulong userId)
        {
            var showServerIdOfWarn = serverId == 0;
            var warns = await GetWarns(serverId, userId);
            var builder = new StringBuilder();

            foreach (var warnEvent in warns)
            {
                builder.Append("\n\nDate: ").Append(Convert.ToString(warnEvent.CreatedAt))
                    .Append("\nGranted by: ").Append(warnEvent.GrantorId)
                    .Append("\nReceiver: ").Append(warnEvent.ReceiverId)
                    .Append("\nReason: ").Append(warnEvent.Reason);

                if (showServerIdOfWarn)
                {
                    builder.Append("\nServer id: ").Append(Convert.ToString(warnEvent.ServerId));
                }
            }

            var result = builder.ToString();
            return String.IsNullOrWhiteSpace(result) ? new string("User has no warns") : result;
        }
    }
}
