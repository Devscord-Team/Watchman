using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Common.Models;
using Watchman.DomainModel.Messages.Services;

namespace Watchman.Web.Server.Areas.Statistics.Models.Dtos
{
    public class PeriodStatisticDto
    {
        public TimeRange TimeRange { get; set; }
        public int Count { get; set; }

        public PeriodStatisticDto()
        {
        }

        public PeriodStatisticDto(PeriodStatistic periodStatistic)
        {
            this.TimeRange = periodStatistic.TimeRange;
            this.Count = periodStatistic.Count;
        }
    }
}
