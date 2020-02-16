using Watchman.Cqrs;

namespace Watchman.DomainModel.Mute.Commands
{
    public class AddMuteEventCommand : ICommand
    {
        public MuteEvent MuteEvent { get; }

        public AddMuteEventCommand(MuteEvent muteEvent)
        {
            MuteEvent = muteEvent;
        }
    }
}
