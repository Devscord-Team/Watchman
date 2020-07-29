using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Responses.Commands
{
    public class RemoveResponsesCommand : ICommand
    {
        public IEnumerable<Response> ResponsesToRemove { get; private set; }
        
        public RemoveResponsesCommand(IEnumerable<Response> responsesToRemove)
        {
            this.ResponsesToRemove = responsesToRemove;
        }
    }
}