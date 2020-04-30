using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Responses.Commands
{
    public class AddResponseCommand : ICommand
    {
        public Response Response { get; private set; }
        public AddResponseCommand(Response response)
        {
            Response = response;
        }
    }
}
