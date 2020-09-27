
using Watchman.Common.Models;

namespace Watchman.Discord.Areas.Statistics.Models
{
    public class StatisticsReportPeriod
    {
        public int MessagesQuantity { get; set; }
        public TimeRange TimeRange { get; set; } //TODO sprawdzenie czy TimeRange odpowiada wartości w Period
    }
}
