using Watchman.Cqrs;

namespace Watchman.DomainModel.Messages.Commands
{
    public class AddPreGeneratedStatisticCommand : ICommand
    {
        public PreGeneratedStatistic PreGeneratedStatistic { get; }

        public AddPreGeneratedStatisticCommand(PreGeneratedStatistic preGeneratedStatistic)
        {
            this.PreGeneratedStatistic = preGeneratedStatistic;
        }
    }
}
