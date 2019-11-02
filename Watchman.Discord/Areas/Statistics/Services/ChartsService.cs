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
            var dates = report.StatisticsPerPeriod.Select(x => x.TimeRange.Start);
            var period = report.StatisticsPerPeriod.First().Period;

            IEnumerable<string> labels = new List<string>();
            switch (period)
            {
                case Period.Hour:
                    labels = dates.Select(x => x.ToString("yyyy-MM-dd HH"));
                    break;
                case Period.Day:
                    labels = dates.Select(x => x.ToString("yyyy-MM-dd"));
                    break;
                case Period.Week:
                    labels = dates.Select(x => x.ToString("yyyy-MM-dd"));
                    break;
                case Period.Month:
                    labels = dates.Select(x => x.ToString("yyyy-MM"));
                    break;
            }

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

            var imagePath = _quickchartService.GetImage(chart);

            return imagePath;
        }


    }
}
