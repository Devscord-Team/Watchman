using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Common.Models;

namespace Watchman.Web.Server.Areas.Commons.Models.Dtos
{
    public class TimeRangeDto
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int DaysBetween { get; set; }

        public TimeRangeDto()
        {
        }

        public TimeRangeDto(TimeRange timeRange)
        {
            this.Start = timeRange.Start;
            this.End = timeRange.End;
            this.DaysBetween = timeRange.DaysBetween;
        }
    }
}
