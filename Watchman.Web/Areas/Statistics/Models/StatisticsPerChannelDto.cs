using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Common.Models;

namespace Watchman.Web.Areas.Statistics.Models
{
    public class StatisticsPerChannelDto
    {
        public string Channel { get; set; }
        public int TotalMessages { get; set; }
    }
}
