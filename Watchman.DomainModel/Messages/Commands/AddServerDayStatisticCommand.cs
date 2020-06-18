using Watchman.Cqrs;

namespace Watchman.DomainModel.Messages.Commands
{
    public class AddServerDayStatisticCommand : ICommand
    {
        public ServerDayStatistic ServerDayStatistic { get; }

        public AddServerDayStatisticCommand(ServerDayStatistic serverDayStatistic) => this.ServerDayStatistic = serverDayStatistic;
    }
}
