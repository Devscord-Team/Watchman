using Watchman.Cqrs;

namespace Watchman.DomainModel.Mute.Commands
{
    public class MarkMuteEventAsUnmutedCommand : ICommand
    {
        public MuteEvent MuteEvent { get; private set; }

        public MarkMuteEventAsUnmutedCommand(MuteEvent muteEvent)
        {
            MuteEvent = muteEvent;
        }
    }
}
