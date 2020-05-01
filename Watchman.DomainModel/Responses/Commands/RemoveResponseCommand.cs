using System;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Responses.Commands
{
    public class RemoveResponseCommand : ICommand
    {
        public RemoveResponseCommand(string onEvent, ulong serverId)
        {
            ServerId = serverId;
            OnEvent = onEvent;
        }

        public ulong ServerId { get; private set; }

        public string OnEvent { get; private set; }
    }
}