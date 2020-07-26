using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Responses.Commands
{
    public class UpdateResponsesCommand : ICommand
    {
        public IEnumerable<Response> ResponsesToUpdate { get; }
        public IEnumerable<Response> ResponsesToAdd { get; }

        public UpdateResponsesCommand(IEnumerable<Response> responsesToUpdate, IEnumerable<Response> responsesToAdd = null)
        {
            this.ResponsesToUpdate = responsesToUpdate;
            this.ResponsesToAdd = responsesToAdd;
        }
    }
}
