using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.CustomCommands.Queries
{
    public class GetCustomCommandsQueryResult : IQueryResult
    {
        public IEnumerable<CustomCommand> CustomCommands { get; private set; }

        public GetCustomCommandsQueryResult(IEnumerable<CustomCommand> customCommands)
        {
            this.CustomCommands = customCommands;
        }
    }
}
