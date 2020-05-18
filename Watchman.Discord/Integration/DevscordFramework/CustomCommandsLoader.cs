using Devscord.DiscordFramework;
using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.Discord.Integration.DevscordFramework
{
    public class CustomCommandsLoader : ICustomCommandsLoader
    {
        private readonly IQueryBus _queryBus;

        public CustomCommandsLoader(IQueryBus queryBus)
        {
            this._queryBus = queryBus;
        }

        public List<CustomCommand> GetCustomCommands()
        {
            throw new NotImplementedException();
        }
    }
}
