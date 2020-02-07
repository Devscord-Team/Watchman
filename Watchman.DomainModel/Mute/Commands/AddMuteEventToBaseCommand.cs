using Watchman.Cqrs;

namespace Watchman.DomainModel.Mute.Commands
{
    public class AddMuteEventToBaseCommand : ICommand
    {
        public MuteEvent MuteEvent { get; }

        public AddMuteEventToBaseCommand(MuteEvent muteEvent)
        {
            MuteEvent = muteEvent;
        }
    }
}
