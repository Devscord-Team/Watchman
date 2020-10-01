using Statsman.Core.TimeSplitting;
using Statsman.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Watchman.Common.Models;
using Watchman.Cqrs;
using Watchman.DomainModel.Messages;
using Watchman.DomainModel.Messages.Queries;
using Watchman.Integrations.Quickchart;

namespace Statsman
{
    public class StatisticsGenerator
    {
        private readonly IQueryBus queryBus;
        private readonly TimeSplittingService timeSplittingService = new TimeSplittingService(); //TODO IoC
        private readonly ChartsService _chartsService = new ChartsService();

        public StatisticsGenerator(IQueryBus queryBus)
        {
            this.queryBus = queryBus;
        }

        public async Task<Stream> PerHour(ulong serverId, TimeSpan timeBehind)
        {
            var messages = await this.GetMessages(serverId, timeBehind);
            var timeRange = TimeRange.Create(DateTime.UtcNow.AddHours(-timeBehind.TotalHours), DateTime.UtcNow);
            var statistics = this.timeSplittingService.GetStatisticsPerHour(messages, timeRange);
            return await this._chartsService.GetImageStatisticsPerPeriod(statistics, $"All Messages Statistics | Item per hour | {timeRange}"); //TODO get from configuration
        }

        public async Task<Stream> PerDay(ulong serverId, TimeSpan timeBehind)
        {
            var preCalculatedDays = await this.GetServerDayStatistics(serverId, timeBehind);
            var messages = await this.GetMessages(serverId, TimeSpan.FromHours(24)); //todo configurable time value
            var timeRange = TimeRange.Create(DateTime.Today.AddDays(-timeBehind.TotalDays), DateTime.Today);
            var statistics = this.timeSplittingService.GetStatisticsPerDay(preCalculatedDays, messages, timeRange);
            return await this._chartsService.GetImageStatisticsPerPeriod(statistics, $"All Messages Statistics | Item per day | {timeRange}");
        }

        public async Task<Stream> PerWeek(ulong serverId, TimeSpan timeBehind)
        {
            var preCalculatedDays = await this.GetServerDayStatistics(serverId, timeBehind);
            var messages = await this.GetMessages(serverId, TimeSpan.FromHours(24));
            var timeRange = TimeRange.Create(DateTime.Today.AddDays(-timeBehind.TotalDays), DateTime.Today);
            var statistics = this.timeSplittingService.GetStatisticsPerWeek(preCalculatedDays, messages, timeRange);
            return await this._chartsService.GetImageStatisticsPerPeriod(statistics, $"All Messages Statistics | Item per week | {timeRange}");
        }

        public async Task<Stream> PerMonth(ulong serverId, TimeSpan timeBehind)
        {
            var preCalculatedDays = await this.GetServerDayStatistics(serverId, timeBehind);
            var messages = await this.GetMessages(serverId, TimeSpan.FromHours(24));
            var timeRange = TimeRange.Create(DateTime.Today.AddDays(-timeBehind.TotalDays), DateTime.Today);
            var statistics = this.timeSplittingService.GetStatisticsPerMonth(preCalculatedDays, messages, timeRange);
            return await this._chartsService.GetImageStatisticsPerPeriod(statistics, $"All Messages Statistics | Item per month | {timeRange}");
        }

        public async Task<Stream> PerQuarter(ulong serverId, TimeSpan timeBehind)
        {
            var preCalculatedDays = await this.GetServerDayStatistics(serverId, timeBehind);
            var messages = await this.GetMessages(serverId, TimeSpan.FromHours(24));
            var timeRange = TimeRange.Create(DateTime.Today.AddDays(-timeBehind.TotalDays), DateTime.Today);
            var statistics = this.timeSplittingService.GetStatisticsPerQuarter(preCalculatedDays, messages, timeRange);
            return await this._chartsService.GetImageStatisticsPerPeriod(statistics, $"All Messages Statistics | Item per quarter | {timeRange}");
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
