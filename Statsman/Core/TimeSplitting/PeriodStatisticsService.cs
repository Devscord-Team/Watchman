using Statsman.Core.TimeSplitting.Models;
using Statsman.Core.TimeSplitting.Services;
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

namespace Statsman.Core.TimeSplitting
{
    public partial class PeriodStatisticsService
    {
        private readonly IQueryBus queryBus;
        private readonly ChartsService _chartsService;
        private readonly StatisticsGroupingService _statisticsGroupingService;

        public PeriodStatisticsService(IQueryBus queryBus, ChartsService chartsService, StatisticsGroupingService statisticsGroupingService)
        {
            this.queryBus = queryBus;
            this._chartsService = chartsService;
            this._statisticsGroupingService = statisticsGroupingService;
        }

        public async Task<(Stream Chart, ResultMessage Message)> PerMinute(StatisticsRequest request)
        {
            var timeRange = TimeRange.ToNow(DateTime.UtcNow.AddMinutes(-request.TimeBehind.TotalMinutes));
            var statistics = await this._statisticsGroupingService.GetStatisticsGroupedPerDetailedPeriod(request, timeRange, DetailedPeriod.Minute);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, "Messages per minute"),
                Message: this.GetMessage(request.UserId, request.ChannelId, DetailedPeriod.Minute, timeRange)); //TODO get label and message from configuration
        }

        public async Task<(Stream Chart, ResultMessage Message)> PerHour(StatisticsRequest request)
        {
            var timeRange = TimeRange.ToNow(DateTime.UtcNow.AddHours(-request.TimeBehind.TotalHours));
            var statistics = await this._statisticsGroupingService.GetStatisticsGroupedPerDetailedPeriod(request, timeRange, DetailedPeriod.Hour);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, "Messages per hour"),
                Message: this.GetMessage(request.UserId, request.ChannelId, DetailedPeriod.Hour, timeRange));
        }

        public async Task<(Stream Chart, ResultMessage Message)> PerDay(StatisticsRequest request)
        {
            var timeRange = TimeRange.ToNow(DateTime.Today.AddDays(-request.TimeBehind.TotalDays));
            var statistics = await this._statisticsGroupingService.GetStatisticsGroupedPerDaysPeriod(request, timeRange, DetailedPeriod.Day);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, "Messages per day"),
                Message: this.GetMessage(request.UserId, request.ChannelId, DetailedPeriod.Day, timeRange));
        }

        public async Task<(Stream Chart, ResultMessage Message)> PerWeek(StatisticsRequest request)
        {
            var timeRange = TimeRange.ToNow(DateTime.Today.AddDays(-request.TimeBehind.TotalDays));
            var statistics = await this._statisticsGroupingService.GetStatisticsGroupedPerDaysPeriod(request, timeRange, DetailedPeriod.Week);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, "Messages per week"),
                Message: this.GetMessage(request.UserId, request.ChannelId, DetailedPeriod.Week, timeRange));
        }

        public async Task<(Stream Chart, ResultMessage Message)> PerMonth(StatisticsRequest request)
        {
            var timeRange = TimeRange.ToNow(DateTime.Today.AddDays(-request.TimeBehind.TotalDays));
            var statistics = await this._statisticsGroupingService.GetStatisticsGroupedPerDaysPeriod(request, timeRange, DetailedPeriod.Month);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, "Messages per month"),
                Message: this.GetMessage(request.UserId, request.ChannelId, DetailedPeriod.Month, timeRange));
        }

        public async Task<(Stream Chart, ResultMessage Message)> PerQuarter(StatisticsRequest request)
        {
            var timeRange = TimeRange.ToNow(DateTime.Today.AddDays(-request.TimeBehind.TotalDays));
            var statistics = await this._statisticsGroupingService.GetStatisticsGroupedPerDaysPeriod(request, timeRange, DetailedPeriod.Quarter);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, "Messages per quarter"),
                Message: this.GetMessage(request.UserId, request.ChannelId, DetailedPeriod.Quarter, timeRange));
        }

        private ResultMessage GetMessage(ulong userId, ulong channelId, DetailedPeriod period, TimeRange timeRange)
        {
            return new ResultMessage($"Statistics",
                $"{Enum.GetName(typeof(DetailedPeriod), period)}",
                userId == 0 ? "All users" : $"User <@{userId}>",
                channelId == 0 ? "All channels" : $"Channel <#{channelId}>",
                $"{timeRange}");
        }
    }
}
