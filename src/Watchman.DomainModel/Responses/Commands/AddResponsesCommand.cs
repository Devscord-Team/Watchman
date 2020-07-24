using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Responses.Commands
{
    public class AddResponsesCommand : ICommand
    {
        public IEnumerable<Response> Responses { get; }

        public AddResponsesCommand(IEnumerable<Response> responses)
        {
            this.Responses = responses;
        }
    }
}
