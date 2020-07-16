using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Common.Models;
using Watchman.Discord.Areas.Statistics.Models;
using Watchman.Integrations.Quickchart;
using Watchman.Integrations.Quickchart.Models;

namespace Watchman.Discord.Areas.Statistics.Services
{
    public class ChartsService
    {
        private readonly QuickchartService _quickchartService;

        public ChartsService()
        {
            this._quickchartService = new QuickchartService();
        }

        public async Task<Stream> GetImageStatisticsPerPeriod(StatisticsReport report)
        {
            var dates = report.StatisticsPerPeriod.Select(x => x.TimeRange.Start);
            var period = report.StatisticsPerPeriod.First().Period;

            var labels = period switch
            {
                Period.Hour => dates.Select(x => x.ToString("yyyy-MM-dd HH")),
                Period.Day => dates.Select(x => x.ToString("yyyy-MM-dd")),
                Period.Week => dates.Select(x => x.ToString("yyyy-MM-dd")),
                Period.Month => dates.Select(x => x.ToString("yyyy-MM")),
                _ => new List<string>()
            };

            var chart = new Chart
            {
                Type = "line",
                Labels = labels.Select(x => x.ToString()).Reverse(),
                Data = new Dataset
                {
                    Label = "Messages",
                    Data = report.StatisticsPerPeriod.Select(x => x.MessagesQuantity).Reverse()
                }
            };

            var image = await this._quickchartService.GetImage(chart);
            return image;
        }
    }
}
