using Watchman.DomainModel.Messages.Services;
using Watchman.Web.Areas.Commons.Models.Dtos;

namespace Watchman.Web.Areas.Statistics.Models.Dtos
{
    public class PeriodStatisticDto
    {
        public TimeRangeDto TimeRange { get; set; }
        public int Count { get; set; }

        public PeriodStatisticDto()
        {
        }

        //public PeriodStatisticDto(PeriodStatistic periodStatistic)
        //{
        //    this.TimeRange = new TimeRangeDto(periodStatistic.TimeRange);
        //    this.Count = periodStatistic.Count;
        //}
    }
}
