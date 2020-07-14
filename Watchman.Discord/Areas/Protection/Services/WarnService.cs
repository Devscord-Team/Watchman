using Devscord.DiscordFramework.Middlewares.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Users;
using Watchman.DomainModel.Users.Commands.Handlers;
using Watchman.DomainModel.Users.Queries;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class WarnService
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;

        public WarnService(ICommandBus commandBus, IQueryBus queryBus)
        {
            this._commandBus = commandBus;
            this._queryBus = queryBus;
        }

        public async Task WarnUser(WarnEvent warnEvent)
        {
            var command = new AddWarnEventCommand(warnEvent);
            await _commandBus.ExecuteAsync(command);
        }

        public Task<GetWarnEventsQueryResults> GetWarns(ulong serverId, ulong userId)
        {
            var query = new GetWarnEventsQuery(serverId, userId);
            return _queryBus.ExecuteAsync(query);
        }

        public async Task<string> GetWarnsToString(ulong serverId, ulong userId)
        {
            bool showServerIdOfWarn = serverId == 0;
            string result = string.Empty;
            var warns = (await GetWarns(serverId, userId)).WarnEvents;

            foreach (var warnEvent in warns)
            {
                result += warnEvent.ToString(showServerIdOfWarn);
                result += "\n\n";
            }

            return result == string.Empty ? "User has no warns" : result;
        }
    }
}
