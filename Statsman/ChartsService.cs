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

        public async Task<Stream> GetImageStatisticsPerPeriod(IEnumerable<TimeStatisticItem> statistics, string label)
        {
            var dates = statistics.Select(x => x.Time.Start);
            var labels = statistics.Count() switch
            {
                <= 2 => dates.Select(x => x.ToString("yyyy-MM-dd HH")),
                <= 31 => dates.Select(x => x.ToString("yyyy-MM-dd")),
                _ => dates.Select(x => x.ToString("yyyy-MM"))
            };
            var chart = new Chart
            {
                Type = "line",
                Labels = labels,
                Data = new Dataset
                {
                    Label = label,
                    Data = statistics.Select(x => x.Value)
                }
            };
            var image = await this._quickchartService.GetImage(chart);
            return image;
        }
    }
}
