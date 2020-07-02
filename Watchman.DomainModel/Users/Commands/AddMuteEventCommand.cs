using Watchman.Cqrs;

namespace Watchman.DomainModel.Users.Commands
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
