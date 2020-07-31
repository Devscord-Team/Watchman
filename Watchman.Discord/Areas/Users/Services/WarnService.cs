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
using Watchman.DomainModel.Warns;
using Watchman.DomainModel.Warns.Commands;
using Watchman.DomainModel.Warns.Queries;

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
            return String.IsNullOrWhiteSpace(result) ? new string("User has no warns") : result;
        }
    }
}
