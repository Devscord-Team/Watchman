using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Mute.Commands
{
    public class AddMuteInfoToDbCommand : ICommand
    {
        public MuteEvent MuteEvent { get; }

        public AddMuteInfoToDbCommand(MuteEvent muteEvent)
        {
            MuteEvent = muteEvent;
        }
    }
}
