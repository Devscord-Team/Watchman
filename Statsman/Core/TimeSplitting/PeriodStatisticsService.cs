using Statsman.Core.TimeSplitting.Models;
using Statsman.Core.TimeSplitting.Services;
using Statsman.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Watchman.Common.Extensions;
using Watchman.Common.Models;

namespace Statsman.Core.TimeSplitting
{
    public partial class PeriodStatisticsService
    {
        private readonly ChartsService _chartsService;
        private readonly StatisticsGroupingService _statisticsGroupingService;

        public PeriodStatisticsService(ChartsService chartsService, StatisticsGroupingService statisticsGroupingService)
        {
            this._chartsService = chartsService;
            this._statisticsGroupingService = statisticsGroupingService;
        }

        public Task<(Stream Chart, ResultMessage Message)> PerMinute(StatisticsRequest request)
        {
            return this.GetResult(request, DetailedPeriod.Minute, DateTime.Now);
        }

        public Task<(Stream Chart, ResultMessage Message)> PerHour(StatisticsRequest request)
        {
            return this.GetResult(request, DetailedPeriod.Minute, DateTime.Now);
        }

        public Task<(Stream Chart, ResultMessage Message)> PerDay(StatisticsRequest request)
        {
            var today = DateTime.Today;
            today = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0, DateTimeKind.Local);
            return this.GetResult(request, DetailedPeriod.Day, today);
        }

        public Task<(Stream Chart, ResultMessage Message)> PerWeek(StatisticsRequest request)
        {
            var today = DateTime.Today;
            today = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0, DateTimeKind.Local);
            return this.GetResult(request, DetailedPeriod.Week, today);
        }

        public Task<(Stream Chart, ResultMessage Message)> PerMonth(StatisticsRequest request)
        {
            var today = DateTime.Today;
            today = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0, DateTimeKind.Local);
            return this.GetResult(request, DetailedPeriod.Month, today);
        }

        public Task<(Stream Chart, ResultMessage Message)> PerQuarter(StatisticsRequest request)
        {
            var today = DateTime.Today;
            today = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0, DateTimeKind.Local);
            return this.GetResult(request, DetailedPeriod.Quarter, today);
        }

        private async Task<(Stream Chart, ResultMessage Message)> GetResult(StatisticsRequest request, DetailedPeriod period, DateTime startTimeRangeTimeOfDay)
        {
            var timeRange = TimeRange.ToNow(startTimeRangeTimeOfDay.Add(request.TimeBehind));
            var statistics = await this._statisticsGroupingService.GetStatisticsGroupedPerDetailedPeriod(request, timeRange, period);
            return (Chart: await this._chartsService.GetImageStatisticsPerPeriod(statistics, $"Messages per {Enum.GetName(typeof(DetailedPeriod), period).ToLower()}"),
                Message: this.GetMessage(request.UserId, request.ChannelId, period, timeRange));
        }

        private ResultMessage GetMessage(ulong userId, ulong channelId, DetailedPeriod period, TimeRange timeRange)
        {
            return new ResultMessage($"Statistics",
                $"{Enum.GetName(typeof(DetailedPeriod), period)}",
                userId == 0 ? "All users" : $"User {userId.GetUserMention()}",
                channelId == 0 ? "All channels" : $"Channel {channelId.GetChannelMention()}",
                $"{timeRange}");
        }
    }
}
