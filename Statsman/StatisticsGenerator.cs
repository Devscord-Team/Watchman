using Statsman.Core.TimeSplitting;
using Statsman.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Watchman.Common.Models;
using Watchman.Cqrs;
using Watchman.DomainModel.Messages;
using Watchman.DomainModel.Messages.Queries;

namespace Statsman
{
    public class StatisticsGenerator
    {
        private readonly IQueryBus queryBus;
        private readonly TimeSplittingService timeSplittingService = new TimeSplittingService(); //TODO IoC

        public StatisticsGenerator(IQueryBus queryBus)
        {
            this.queryBus = queryBus;
        }

        public async Task PerHour(ulong serverId, TimeSpan timeBehind)
        {
            var messages = this.GetMessages(serverId, timeBehind);
        }

        public async Task<IEnumerable<TimeStatisticItem>> PerDay(ulong serverId, TimeSpan timeBehind)
        {
            var preCalculatedDays = await this.GetServerDayStatistics(serverId, timeBehind);
            var messages = await this.GetMessages(serverId, TimeSpan.FromHours(24)); //todo configurable time value
            var results = this.timeSplittingService.GetStatisticsPerDay(preCalculatedDays, messages, TimeRange.Create(DateTime.Today.AddDays(-timeBehind.TotalDays), DateTime.Today));
            return results;
        }

        public async Task PerWeek(ulong serverId, TimeSpan timeBehind)
        {
            var preCalculatedDays = this.GetServerDayStatistics(serverId, timeBehind);
            var messages = this.GetMessages(serverId, TimeSpan.FromHours(24));
        }

        public async Task PerMonth(ulong serverId, TimeSpan timeBehind)
        {
            var preCalculatedDays = this.GetServerDayStatistics(serverId, timeBehind);
            var messages = this.GetMessages(serverId, TimeSpan.FromHours(24));
        }

        public async Task PerQuarter(ulong serverId, TimeSpan timeBehind)
        {
            var preCalculatedDays = this.GetServerDayStatistics(serverId, timeBehind);
            var messages = this.GetMessages(serverId, TimeSpan.FromHours(24));
        }

        private async Task<IEnumerable<Message>> GetMessages(ulong serverId, TimeSpan timeBehind)
        {
            var query = new GetMessagesQuery(serverId)
            {
                SentDate = TimeRange.Create(DateTime.UtcNow.AddHours(-timeBehind.TotalHours), DateTime.UtcNow)
            };
            return (await this.queryBus.ExecuteAsync(query)).Messages.ToList();
        }

        private async Task<IEnumerable<ServerDayStatistic>> GetServerDayStatistics(ulong serverId, TimeSpan timeBehind)
        {
            var query = new GetServerDayStatisticsQuery(serverId)
            {
                SentDate = TimeRange.Create(DateTime.Today.AddDays(-timeBehind.TotalDays), DateTime.Today)
            };
            return (await this.queryBus.ExecuteAsync(query)).ServerDayStatistics.ToList();
        }
    }
}
