using System;
using Watchman.Common.Models;

namespace Watchman.Web.Areas.Commons.Models.Dtos
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
            Start = timeRange.Start;
            End = timeRange.End;
            DaysBetween = timeRange.DaysBetween;
        }
    }
}
