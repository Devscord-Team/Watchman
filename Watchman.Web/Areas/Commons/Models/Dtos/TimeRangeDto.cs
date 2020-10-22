using System;
using TypeGen.Core.TypeAnnotations;
using Watchman.Common.Models;

namespace Watchman.Web.Areas.Commons.Models.Dtos
{
    [ExportTsClass(OutputDir = "ClientApp/src/models")]
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
