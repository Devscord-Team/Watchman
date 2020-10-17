using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Messages.Commands.Handlers
{
    public class AddOrUpdatePreGeneratedStatisticsCommandHandler : ICommandHandler<AddOrUpdatePreGeneratedStatisticsCommand>
    {
        private ISessionFactory _sessionFactory;

        public AddOrUpdatePreGeneratedStatisticsCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(AddOrUpdatePreGeneratedStatisticsCommand command)
        {
            using var session = this._sessionFactory.CreateLite();
            var preGeneratedStatisticsInRepository = session.Get<PreGeneratedStatistic>().ToList();

            var (toAdd, toUpdate) = this.CategorizeStatistics(command.PreGeneratedStatistics, preGeneratedStatisticsInRepository);

            var toAddTask = session.AddAsync(toAdd);
            var toUpdateTasks = toUpdate.Select(x => session.UpdateAsync(x)).ToArray();
            await toAddTask;
            Task.WaitAll(toUpdateTasks);
        }

        private (IEnumerable<PreGeneratedStatistic> toAdd, IEnumerable<PreGeneratedStatistic> toUpdate) CategorizeStatistics(IEnumerable<PreGeneratedStatistic> newStatistics, IEnumerable<PreGeneratedStatistic> statisticsInRepository)
        {
            var statisticsToAdd = new List<PreGeneratedStatistic>();
            var statisticsToUpdate = new List<PreGeneratedStatistic>();

            foreach (var statistic in newStatistics)
            {
                var similarStatistic = statisticsInRepository
                .FirstOrDefault(x =>
                    x.ServerId == statistic.ServerId && x.UserId == statistic.UserId && x.ChannelId == statistic.ChannelId
                    && x.Period == statistic.Period && x.TimeRange == statistic.TimeRange);

                if (similarStatistic == null)
                {
                    statisticsToAdd.Add(statistic);
                    continue;
                }
                var version = similarStatistic.Version;
                similarStatistic.SetCount(statistic.Count);
                if (version == similarStatistic.Version)
                {
                    continue;
                }
                statisticsToUpdate.Add(similarStatistic);
            }
            return (toAdd: statisticsToAdd, toUpdate: statisticsToUpdate);
        }
    }
}
