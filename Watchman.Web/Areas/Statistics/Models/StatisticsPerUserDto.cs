using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Watchman.Web.Areas.Statistics.Models
{
    public class StatisticsPerUserDto
    {
        public string User { get; set; }
        public int TotalMessages { get; set; }
        //there should be lazy loading
        public IEnumerable<StatisticsPerPeriodDto> StatisticsPerPeriod { get; set; }
        public IEnumerable<StatisticsPerChannelDto> StatisticsPerChannel { get; set; }
        public IEnumerable<StatisticsPerPeriodAndChannelDto> StatisticsPerPeriodAndChannel { get; set; }
    }
}
