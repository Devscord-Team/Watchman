using Devscord.DiscordFramework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.CustomCommands.Queries;

namespace Watchman.Discord.Integration.DevscordFramework
{
    public class CustomCommandsLoader : ICustomCommandsLoader
    {
        private readonly IQueryBus _queryBus;

        public CustomCommandsLoader(IQueryBus queryBus)
        {
            this._queryBus = queryBus;
        }

        public async Task<List<CustomCommand>> GetCustomCommands()
        {
            var query = new GetCustomCommandsQuery();
            return await this._queryBus.ExecuteAsync(query);
        }
    }
}
