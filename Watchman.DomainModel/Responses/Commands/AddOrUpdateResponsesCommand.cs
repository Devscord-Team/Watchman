using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Responses.Commands
{
    public class AddOrUpdateResponsesCommand : ICommand
    {
        public IEnumerable<Response> ResponsesToAddOrUpdate { get; private set; }

        public AddOrUpdateResponsesCommand(IEnumerable<Response> responsesToAddOrUpdate)
        {
            this.ResponsesToAddOrUpdate = responsesToAddOrUpdate;
        }
    }
}
