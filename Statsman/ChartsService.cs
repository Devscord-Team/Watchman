using Statsman.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Watchman.Integrations.Quickchart;
using Watchman.Integrations.Quickchart.Models;

namespace Statsman
{
    public class ChartsService
    {
        private readonly QuickchartService _quickchartService;

        public ChartsService()
        {
            this._quickchartService = new QuickchartService();
        }

        public async Task<Stream> GetImageStatisticsPerPeriod(IEnumerable<TimeStatisticItem> statistics)
        {
            var dates = statistics.Select(x => x.Time.Start);
            var labels = statistics.Count() switch
            {
                int x when x <= 1 => dates.Select(x => x.ToString("yyyy-MM-dd HH")),
                int x when x <= 30 => dates.Select(x => x.ToString("yyyy-MM-dd")),
                int x when x > 30 => dates.Select(x => x.ToString("yyyy-MM")),
                _ => new List<string>()
            };
            var chart = new Chart
            {
                Type = "line",
                Labels = labels.Select(x => x.ToString()).Reverse(),
                Data = new Dataset
                {
                    Label = "Messages",
                    Data = statistics.Select(x => x.Value).Reverse()
                }
            };
            var image = await this._quickchartService.GetImage(chart);
            return image;
        }
    }
}
