using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Users.Commands.Handlers
{
    public class AddWarnEventCommand : ICommand
    {
        public WarnEvent WarnEvent { get; }

        public AddWarnEventCommand(WarnEvent warnEvent)
        {
            this.WarnEvent = warnEvent;
        }
    }
}
