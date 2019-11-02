using System.Collections.Generic;
using Watchman.Common.Models;

namespace Watchman.Discord.Areas.Statistics.Models
{
    public class StatisticsReport
    {
        public int AllMessages { get; set; }
        public IEnumerable<StatisticsReportPeriod> StatisticsPerPeriod { get; set; }
        public TimeRange TimeRange { get; set; }
    }
}
