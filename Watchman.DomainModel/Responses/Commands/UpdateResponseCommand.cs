using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Responses.Commands
{
    public class UpdateResponseCommand : ICommand
    {
        public Guid Id { get; set; }
        public string Message { get; }

        public UpdateResponseCommand(Guid id, string message)
        {
            Id = id;
            Message = message;
        }
    }
}
