using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Common.Models;
using Watchman.DomainModel.Messages.Services;
using Watchman.Web.Server.Areas.Commons.Models.Dtos;

namespace Watchman.Web.Server.Areas.Statistics.Models.Dtos
{
    public class PeriodStatisticDto
    {
        public TimeRangeDto TimeRange { get; set; }
        public int Count { get; set; }

        public PeriodStatisticDto()
        {
        }

        public PeriodStatisticDto(PeriodStatistic periodStatistic)
        {
            this.TimeRange = new TimeRangeDto(periodStatistic.TimeRange);
            this.Count = periodStatistic.Count;
        }
    }
}
