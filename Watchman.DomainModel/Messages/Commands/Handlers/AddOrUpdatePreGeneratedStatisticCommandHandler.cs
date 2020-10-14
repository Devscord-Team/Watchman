using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Messages.Commands.Handlers
{
    public class AddOrUpdatePreGeneratedStatisticCommandHandler : ICommandHandler<AddOrUpdatePreGeneratedStatisticCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public AddOrUpdatePreGeneratedStatisticCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(AddOrUpdatePreGeneratedStatisticCommand command)
        {
            using var session = this._sessionFactory.Create();
            var currentTimeRange = session.Get<PreGeneratedStatistic>()
                .FirstOrDefault(x =>
                x.ServerId == command.PreGeneratedStatistic.ServerId
                && x.UserId == command.PreGeneratedStatistic.ServerId
                && x.ChannelId == command.PreGeneratedStatistic.ServerId
                && x.Period == command.PreGeneratedStatistic.Period
                && x.TimeRange == command.PreGeneratedStatistic.TimeRange);
            if(currentTimeRange == null)
            {
                await session.AddAsync(command.PreGeneratedStatistic);
                return;
            }
            currentTimeRange.SetCount(command.PreGeneratedStatistic.Count);
            await session.UpdateAsync(currentTimeRange);
        }
    }
}
