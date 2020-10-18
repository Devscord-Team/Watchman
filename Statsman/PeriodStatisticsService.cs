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
    public class PeriodStatisticsService
    {
        private readonly IQueryBus queryBus;
        private readonly TimeSplittingService timeSplittingService = new TimeSplittingService(); //TODO IoC
        private readonly ChartsService _chartsService = new ChartsService();

        public PeriodStatisticsService(IQueryBus queryBus)
        {
            this.queryBus = queryBus;
        }

        public async Task<(Stream Chart, string Message)> PerMinute(StatisticsRequest request)
        {
            var timeRange = TimeRange.Create(DateTime.UtcNow.AddMinutes(-request.TimeBehind.TotalMinutes), DateTime.UtcNow);
            var statistics = await this.GetStatisticsGroupedPerDetailedPeriod(request, timeRange, Period.Minute);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, "Messages per minute"),
                Message: this.GetMessage(request.UserId, request.ChannelId, Period.Minute, timeRange)); //TODO get label and message from configuration
        }

        public async Task<(Stream Chart, string Message)> PerHour(StatisticsRequest request)
        {
            var timeRange = TimeRange.Create(DateTime.UtcNow.AddHours(-request.TimeBehind.TotalHours), DateTime.UtcNow);
            var statistics = await this.GetStatisticsGroupedPerDetailedPeriod(request, timeRange, Period.Hour);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, "Messages per hour"),
                Message: this.GetMessage(request.UserId, request.ChannelId, Period.Hour, timeRange));
        }

        public async Task<(Stream Chart, string Message)> PerDay(StatisticsRequest request)
        {
            var timeRange = TimeRange.Create(DateTime.Today.AddDays(-request.TimeBehind.TotalDays), DateTime.UtcNow);
            var statistics = await this.GetStatisticsGroupedPerDaysPeriod(request, timeRange, Period.Day);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, "Messages per day"),
                Message: this.GetMessage(request.UserId, request.ChannelId, Period.Day, timeRange));
        }

        public async Task<(Stream Chart, string Message)> PerWeek(StatisticsRequest request)
        {
            var timeRange = TimeRange.Create(DateTime.Today.AddDays(-request.TimeBehind.TotalDays), DateTime.UtcNow);
            var statistics = await this.GetStatisticsGroupedPerDaysPeriod(request, timeRange, Period.Week);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, "Messages per week"),
                Message: this.GetMessage(request.UserId, request.ChannelId, Period.Week, timeRange));
        }

        public async Task<(Stream Chart, string Message)> PerMonth(StatisticsRequest request)
        {
            var timeRange = TimeRange.Create(DateTime.Today.AddDays(-request.TimeBehind.TotalDays), DateTime.UtcNow);
            var statistics = await this.GetStatisticsGroupedPerDaysPeriod(request, timeRange, Period.Month);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, "Messages per month"),
                Message: this.GetMessage(request.UserId, request.ChannelId, Period.Month, timeRange));
        }

        public async Task<(Stream Chart, string Message)> PerQuarter(StatisticsRequest request)
        {
            var timeRange = TimeRange.Create(DateTime.Today.AddDays(-request.TimeBehind.TotalDays), DateTime.UtcNow);
            var statistics = await this.GetStatisticsGroupedPerDaysPeriod(request, timeRange, Period.Quarter);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, "Messages per quarter"), 
                Message: this.GetMessage(request.UserId, request.ChannelId, Period.Quarter, timeRange));
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
            var preGeneratedStatistics = await this.GetPreGeneratedStatistics(statisticsRequest, timeRange);
            var messages = await this.GetMessages(statisticsRequest, TimeSpan.FromHours(48));
            return period switch
            {
                Period.Day => this.timeSplittingService.GetStatisticsPerDay(preGeneratedStatistics, messages, timeRange),
                Period.Week => this.timeSplittingService.GetStatisticsPerWeek(preGeneratedStatistics, messages, timeRange),
                Period.Month => this.timeSplittingService.GetStatisticsPerMonth(preGeneratedStatistics, messages, timeRange),
                Period.Quarter => this.timeSplittingService.GetStatisticsPerQuarter(preGeneratedStatistics, messages, timeRange),
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

        private async Task<IEnumerable<PreGeneratedStatistic>> GetPreGeneratedStatistics(StatisticsRequest statisticsRequest, TimeRange timeRange)
        {
            var query = new GetPreGeneratedStatisticQuery(statisticsRequest.ServerId, statisticsRequest.ChannelId, statisticsRequest.UserId, timeRange: timeRange); //only for day //TODO - get also for quarter and month
            return (await this.queryBus.ExecuteAsync(query)).PreGeneratedStatistics.ToList();
        }

        private string GetMessage(ulong userId, ulong channelId, Period period, TimeRange timeRange)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"{Enum.GetName(typeof(Period), period)} statistics for:");
            stringBuilder.AppendLine(userId == 0 ? "All users" : $"User <@{userId}>");
            stringBuilder.AppendLine(channelId == 0 ? "All channels" : $"Channel <#{channelId}>");
            stringBuilder.AppendLine($"In time range: {timeRange}");
            return stringBuilder.ToString();
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
