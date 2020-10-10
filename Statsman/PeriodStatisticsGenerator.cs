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
    public class PeriodStatisticsGenerator
    {
        private readonly IQueryBus queryBus;
        private readonly TimeSplittingService timeSplittingService = new TimeSplittingService(); //TODO IoC
        private readonly ChartsService _chartsService = new ChartsService();

        public PeriodStatisticsGenerator(IQueryBus queryBus)
        {
            this.queryBus = queryBus;
        }

        public async Task<(Stream Chart, string Message)> PerMinute(StatisticsRequest request)
        {
            var timeRange = TimeRange.Create(DateTime.UtcNow.AddMinutes(-request.TimeBehind.TotalMinutes), DateTime.UtcNow);
            var statistics = await this.GetStatisticsGroupedPerDetailedPeriod(request.ServerId, timeRange, Period.Minute);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, "Messages per minute"),
                Message: $"All Messages Statistics\r\nItem per minute\r\n{timeRange}"); //TODO get label and message from configuration
        }

        public async Task<(Stream Chart, string Message)> PerHour(StatisticsRequest request)
        {
            var timeRange = TimeRange.Create(DateTime.UtcNow.AddHours(-request.TimeBehind.TotalHours), DateTime.UtcNow);
            var statistics = await this.GetStatisticsGroupedPerDetailedPeriod(request.ServerId, timeRange, Period.Hour);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, "Messages per hour"), 
                Message: $"All Messages Statistics\r\nItem per hour\r\n{timeRange}");
        }

        public async Task<(Stream Chart, string Message)> PerDay(StatisticsRequest request)
        {
            var timeRange = TimeRange.Create(DateTime.Today.AddDays(-request.TimeBehind.TotalDays), DateTime.Today);
            var statistics = await this.GetStatisticsGroupedPerDaysPeriod(request.ServerId, timeRange, Period.Day);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, "Messages per day"), 
                Message: $"All Messages Statistics\r\nItem per day\r\n{timeRange}");
        }

        public async Task<(Stream Chart, string Message)> PerWeek(StatisticsRequest request)
        {
            var timeRange = TimeRange.Create(DateTime.Today.AddDays(-request.TimeBehind.TotalDays), DateTime.Today);
            var statistics = await this.GetStatisticsGroupedPerDaysPeriod(request.ServerId, timeRange, Period.Week);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, "Messages per week"), 
                Message: $"All Messages Statistics\r\nItem per week\r\n{timeRange}");
        }

        public async Task<(Stream Chart, string Message)> PerMonth(StatisticsRequest request)
        {
            var timeRange = TimeRange.Create(DateTime.Today.AddDays(-request.TimeBehind.TotalDays), DateTime.Today);
            var statistics = await this.GetStatisticsGroupedPerDaysPeriod(request, timeRange, Period.Month);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, "Messages per month"), 
                Message: $"All Messages Statistics\r\nItem per month\r\n{timeRange}");
        }

        public async Task<(Stream Chart, string Message)> PerQuarter(StatisticsRequest request)
        {
            var timeRange = TimeRange.Create(DateTime.Today.AddDays(-request.TimeBehind.TotalDays), DateTime.Today);
            var statistics = await this.GetStatisticsGroupedPerDaysPeriod(request, timeRange, Period.Quarter);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, "Messages per quarter"), 
                Message: $"All Messages Statistics\r\nItem per quarter\r\n{timeRange}");
        }

        private async Task<IEnumerable<TimeStatisticItem>> GetStatisticsGroupedPerDetailedPeriod(StatisticsRequest statisticsRequest, TimeRange timeRange, Period period)
        {
            var messages = await this.GetMessages(statisticsRequest, timeRange);
            return period switch
            {
                Period.Minute => this.timeSplittingService.GetStatisticsPerMinute(messages, timeRange),
                Period.Hour => this.timeSplittingService.GetStatisticsPerHour(messages, timeRange),
                _ => throw new NotImplementedException()
            };
        }

        private async Task<IEnumerable<TimeStatisticItem>> GetStatisticsGroupedPerDaysPeriod(StatisticsRequest statisticsRequest, TimeRange timeRange, Period period)
        {
            var preCalculatedDays = await this.GetServerDayStatistics(statisticsRequest, timeRange);
            var messages = await this.GetMessages(statisticsRequest, TimeSpan.FromHours(24));
            return period switch
            {
                Period.Day => this.timeSplittingService.GetStatisticsPerDay(preCalculatedDays, messages, timeRange),
                Period.Week => this.timeSplittingService.GetStatisticsPerWeek(preCalculatedDays, messages, timeRange),
                Period.Month => this.timeSplittingService.GetStatisticsPerMonth(preCalculatedDays, messages, timeRange),
                Period.Quarter => this.timeSplittingService.GetStatisticsPerQuarter(preCalculatedDays, messages, timeRange),
                _ => throw new NotImplementedException()
            };
        }

        private async Task<IEnumerable<Message>> GetMessages(StatisticsRequest statisticsRequest, TimeSpan timeBehind)
        {
            return await this.GetMessages(statisticsRequest, TimeRange.Create(DateTime.UtcNow.AddHours(-timeBehind.TotalHours), DateTime.UtcNow));
        }

        private async Task<IEnumerable<Message>> GetMessages(StatisticsRequest statisticsRequest, TimeRange timeRange)
        {
            var query = new GetMessagesQuery(statisticsRequest.ServerId, statisticsRequest.ChannelId, statisticsRequest.UserId)
            {
                SentDate = timeRange
            };
            return (await this.queryBus.ExecuteAsync(query)).Messages.ToList();
        }

        private async Task<IEnumerable<ServerDayStatistic>> GetServerDayStatistics(StatisticsRequest statisticsRequest, TimeRange timeRange)
        {
            var query = new GetPreGeneratedStatisticQuery(statisticsRequest.ServerId)
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
