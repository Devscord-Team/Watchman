using System.Collections.Generic;
using Watchman.Discord.Areas.Commons.Models;

namespace Watchman.Discord.Areas.Statistics.Models
{
    public class StatisticsReport
    {
        public int AllMessages { get; set; }
        public IEnumerable<StatisticsReportPeriod> Periods { get; set; }
        public TimeRange TimeRange { get; set; }
    }

    public class StatisticsReportPeriod
    {
        public int MessagesQuantity { get; set; }
        public Period Period { get; set; }
        public TimeRange TimeRange { get; set; } //TODO sprawdzenie czy TimeRange odpowiada wartości w Period
    }
}
