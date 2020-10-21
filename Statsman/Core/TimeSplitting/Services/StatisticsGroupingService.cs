using Statsman.Core.TimeSplitting.Models;
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

namespace Statsman.Core.TimeSplitting.Services
{
    public class StatisticsGroupingService
    {
        private readonly IQueryBus _queryBus;
        private readonly TimeSplittingService _timeSplittingService;

        public StatisticsGroupingService(IQueryBus queryBus, TimeSplittingService timeSplittingService)
        {
            this._queryBus = queryBus;
            this._timeSplittingService = timeSplittingService;
        }

        public async Task<IEnumerable<TimeStatisticItem>> GetStatisticsGroupedPerDetailedPeriod(StatisticsRequest statisticsRequest, TimeRange timeRange, DetailedPeriod period)
        {
            var messages = await this.GetMessages(statisticsRequest, timeRange);
            return period switch
            {
                DetailedPeriod.Minute => this._timeSplittingService.GetStatisticsPerMinute(messages, timeRange),
                DetailedPeriod.Hour => this._timeSplittingService.GetStatisticsPerHour(messages, timeRange),
                _ => throw new NotImplementedException()
            };
        }

        public async Task<IEnumerable<TimeStatisticItem>> GetStatisticsGroupedPerDaysPeriod(StatisticsRequest statisticsRequest, TimeRange timeRange, DetailedPeriod period)
        {
            var preGeneratedStatistics = await this.GetPreGeneratedStatistics(statisticsRequest, timeRange);
            var messages = await this.GetMessages(statisticsRequest, TimeSpan.FromHours(48));
            return period switch
            {
                DetailedPeriod.Day => this._timeSplittingService.GetStatisticsPerDay(preGeneratedStatistics, messages, timeRange),
                DetailedPeriod.Week => this._timeSplittingService.GetStatisticsPerWeek(preGeneratedStatistics, messages, timeRange),
                DetailedPeriod.Month => this._timeSplittingService.GetStatisticsPerMonth(preGeneratedStatistics, messages, timeRange),
                DetailedPeriod.Quarter => this._timeSplittingService.GetStatisticsPerQuarter(preGeneratedStatistics, messages, timeRange),
                _ => throw new NotImplementedException()
            };
        }

        private Task<IReadOnlyList<Message>> GetMessages(StatisticsRequest statisticsRequest, TimeSpan timeBehind)
        {
            return this.GetMessages(statisticsRequest, TimeRange.ToNow(DateTime.UtcNow.AddHours(-timeBehind.TotalHours)));
        }

        private async Task<IReadOnlyList<Message>> GetMessages(StatisticsRequest statisticsRequest, TimeRange timeRange)
        {
            var query = new GetMessagesQuery(statisticsRequest.ServerId, statisticsRequest.ChannelId, statisticsRequest.UserId)
            {
                SentDate = timeRange
            };
            return (await this._queryBus.ExecuteAsync(query)).Messages.ToList();
        }

        private async Task<IReadOnlyList<PreGeneratedStatistic>> GetPreGeneratedStatistics(StatisticsRequest statisticsRequest, TimeRange timeRange)
        {
            var query = new GetPreGeneratedStatisticQuery(statisticsRequest.ServerId, statisticsRequest.ChannelId, statisticsRequest.UserId, timeRange: timeRange); //only for day //TODO - get also for quarter and month
            return (await this._queryBus.ExecuteAsync(query)).PreGeneratedStatistics.ToList();
        }
    }
}
