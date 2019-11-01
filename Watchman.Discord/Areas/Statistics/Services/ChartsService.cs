using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
            _quickchartService = new QuickchartService();
        }

        public string GetImageStatisticsPerPeriod(StatisticsReport report)
        {
            var chart = new Chart
            {
                Type = "line",
                Labels = report.StatisticsPerPeriod.Select(x => x.TimeRange.Start.ToString()),
                Data = new Dataset
                {
                    Label = "Messages",
                    Data = report.StatisticsPerPeriod.Select(x => x.MessagesQuantity)
                }
            };

            var imagePath = _quickchartService.GetImage(chart);

            return imagePath;
        }


    }
}
