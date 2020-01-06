using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Common.Models;

namespace Watchman.Web.Areas.Statistics.Models
{
    public class StatisticsPerPeriodDto
    {
        public Period Period { get; set; }
        public TimeRange TimeRange { get; set; }
        public int TotalMessages { get; set; }
    }
}
