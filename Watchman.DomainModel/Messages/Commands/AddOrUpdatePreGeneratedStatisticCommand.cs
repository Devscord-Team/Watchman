using Watchman.Cqrs;

namespace Watchman.DomainModel.Messages.Commands
{
    public class AddOrUpdatePreGeneratedStatisticCommand : ICommand
    {
        public PreGeneratedStatistic PreGeneratedStatistic { get; }

        public AddOrUpdatePreGeneratedStatisticCommand(PreGeneratedStatistic preGeneratedStatistic)
        {
            this.PreGeneratedStatistic = preGeneratedStatistic;
        }
    }
}
