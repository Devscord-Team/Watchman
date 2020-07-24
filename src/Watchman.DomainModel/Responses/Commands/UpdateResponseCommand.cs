using System;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Responses.Commands
{
    public class UpdateResponseCommand : ICommand
    {
        public Guid Id { get; set; }
        public string Message { get; }

        public UpdateResponseCommand(Guid id, string message)
        {
            this.Id = id;
            this.Message = message;
        }
    }
}
