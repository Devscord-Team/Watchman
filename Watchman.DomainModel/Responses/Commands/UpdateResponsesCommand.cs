using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Responses.Commands
{
    public class UpdateResponsesCommand : ICommand
    {
        public IEnumerable<Response> Responses { get; }

        public UpdateResponsesCommand(IEnumerable<Response> responses)
        {
            this.Responses = responses;
        }
    }
}
