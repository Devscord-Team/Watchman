using Watchman.Cqrs;
using Watchman.DomainModel.Muting;

namespace Watchman.DomainModel.Muting.Commands
{
    public class AddMuteEventCommand : ICommand
    {
        public MuteEvent MuteEvent { get; }

        public AddMuteEventCommand(MuteEvent muteEvent)
        {
            this.MuteEvent = muteEvent;
        }
    }
}
