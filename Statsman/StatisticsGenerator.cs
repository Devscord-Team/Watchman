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

        public async Task<(Stream Chart, string Message)> PerMinute(ulong serverId, TimeSpan timeBehind)
        {
            var timeRange = TimeRange.Create(DateTime.UtcNow.AddMinutes(-timeBehind.TotalMinutes), DateTime.UtcNow);
            var statistics = await this.GetStatisticsGroupedPerDetailedPeriod(serverId, timeRange, Period.Minute);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, "Messages per minute"),
                Message: $"All Messages Statistics\r\nItem per minute\r\n{timeRange}"); //TODO get label and message from configuration
        }

        public async Task<(Stream Chart, string Message)> PerHour(ulong serverId, TimeSpan timeBehind)
        {
            var timeRange = TimeRange.Create(DateTime.UtcNow.AddHours(-timeBehind.TotalHours), DateTime.UtcNow);
            var statistics = await this.GetStatisticsGroupedPerDetailedPeriod(serverId, timeRange, Period.Hour);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, "Messages per hour"), 
                Message: $"All Messages Statistics\r\nItem per hour\r\n{timeRange}");
        }

        public async Task<(Stream Chart, string Message)> PerDay(ulong serverId, TimeSpan timeBehind)
        {
            var timeRange = TimeRange.Create(DateTime.Today.AddDays(-timeBehind.TotalDays), DateTime.Today);
            var statistics = await this.GetStatisticsGroupedPerDaysPeriod(serverId, timeRange, Period.Day);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, "Messages per day"), 
                Message: $"All Messages Statistics\r\nItem per day\r\n{timeRange}");
        }

        public async Task<(Stream Chart, string Message)> PerWeek(ulong serverId, TimeSpan timeBehind)
        {
            var timeRange = TimeRange.Create(DateTime.Today.AddDays(-timeBehind.TotalDays), DateTime.Today);
            var statistics = await this.GetStatisticsGroupedPerDaysPeriod(serverId, timeRange, Period.Week);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, "Messages per week"), 
                Message: $"All Messages Statistics\r\nItem per week\r\n{timeRange}");
        }

        public async Task<(Stream Chart, string Message)> PerMonth(ulong serverId, TimeSpan timeBehind)
        {
            var timeRange = TimeRange.Create(DateTime.Today.AddDays(-timeBehind.TotalDays), DateTime.Today);
            var statistics = await this.GetStatisticsGroupedPerDaysPeriod(serverId, timeRange, Period.Month);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, "Messages per month"), 
                Message: $"All Messages Statistics\r\nItem per month\r\n{timeRange}");
        }

        public async Task<(Stream Chart, string Message)> PerQuarter(ulong serverId, TimeSpan timeBehind)
        {
            var timeRange = TimeRange.Create(DateTime.Today.AddDays(-timeBehind.TotalDays), DateTime.Today);
            var statistics = await this.GetStatisticsGroupedPerDaysPeriod(serverId, timeRange, Period.Quarter);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, "Messages per quarter"), 
                Message: $"All Messages Statistics\r\nItem per quarter\r\n{timeRange}");
        }

        private async Task<IEnumerable<TimeStatisticItem>> GetStatisticsGroupedPerDetailedPeriod(ulong serverId, TimeRange timeRange, Period period)
        {
            var messages = await this.GetMessages(serverId, timeRange);
            return period switch
            {
                Period.Minute => this.timeSplittingService.GetStatisticsPerMinute(messages, timeRange),
                Period.Hour => this.timeSplittingService.GetStatisticsPerHour(messages, timeRange),
                _ => throw new NotImplementedException()
            };
        }

        private async Task<IEnumerable<TimeStatisticItem>> GetStatisticsGroupedPerDaysPeriod(ulong serverId, TimeRange timeRange, Period period)
        {
            var preCalculatedDays = await this.GetServerDayStatistics(serverId, timeRange);
            var messages = await this.GetMessages(serverId, TimeSpan.FromHours(24));
            return period switch
            {
                Period.Day => this.timeSplittingService.GetStatisticsPerDay(preCalculatedDays, messages, timeRange),
                Period.Week => this.timeSplittingService.GetStatisticsPerWeek(preCalculatedDays, messages, timeRange),
                Period.Month => this.timeSplittingService.GetStatisticsPerMonth(preCalculatedDays, messages, timeRange),
                Period.Quarter => this.timeSplittingService.GetStatisticsPerQuarter(preCalculatedDays, messages, timeRange),
                _ => throw new NotImplementedException()
            };
        }

        private async Task<IEnumerable<Message>> GetMessages(ulong serverId, TimeSpan timeBehind)
        {
            return await this.GetMessages(serverId, TimeRange.Create(DateTime.UtcNow.AddHours(-timeBehind.TotalHours), DateTime.UtcNow));
        }

        private async Task<IEnumerable<Message>> GetMessages(ulong serverId, TimeRange timeRange)
        {
            var query = new GetMessagesQuery(serverId)
            {
                SentDate = timeRange
            };
            return (await this.queryBus.ExecuteAsync(query)).Messages.ToList();
        }

        private async Task<IEnumerable<ServerDayStatistic>> GetServerDayStatistics(ulong serverId, TimeRange timeRange)
        {
            var query = new GetServerDayStatisticsQuery(serverId)
            {
                SentDate = timeRange
            };
            return (await this.queryBus.ExecuteAsync(query)).ServerDayStatistics.ToList();
        }

        private enum Period
        {
            Minute = 1,
            Hour = 2,
            Day = 4,
            Week = 8,
            Month = 16,
            Quarter = 32
        }
    }
}
