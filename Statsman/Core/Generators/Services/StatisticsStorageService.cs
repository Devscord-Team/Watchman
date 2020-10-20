using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Watchman.Common.Models;
using Watchman.Cqrs;
using Watchman.DomainModel.Messages;
using Watchman.DomainModel.Messages.Commands;

namespace Statsman.Core.Generators.Services
{
    public class StatisticsStorageService
    {
        private List<PreGeneratedStatistic> _preGeneratedStatistics = new List<PreGeneratedStatistic>();
        private readonly ICommandBus _commandBus;

        public StatisticsStorageService(ICommandBus commandBus)
        {
            this._commandBus = commandBus;
        }

        public void SaveStatisticCommand(ulong serverId, ulong userId, ulong channelId, int count, TimeRange timeRange, string period) //todo test
        {
            var preGeneratedStatistic = new PreGeneratedStatistic(serverId, count, timeRange, period);
            preGeneratedStatistic.SetUser(userId);
            preGeneratedStatistic.SetChannel(channelId);
            this._preGeneratedStatistics.Add(preGeneratedStatistic);
        }

        public async Task SaveChanges()
        {
            var command = new AddOrUpdatePreGeneratedStatisticsCommand(this._preGeneratedStatistics);
            await this._commandBus.ExecuteAsync(command);
            this._preGeneratedStatistics = new List<PreGeneratedStatistic>();
        }
    }
}
