using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Responses.Commands
{
    public class AddResponsesCommand : ICommand
    {
        public IEnumerable<Response> Responses { get; }

        public AddResponsesCommand(IEnumerable<Response> responses)
        {
            Responses = responses;
        }
    }
}
