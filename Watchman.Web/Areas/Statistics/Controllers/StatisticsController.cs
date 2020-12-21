using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Common.Models;
using Watchman.Cqrs;
using Watchman.DomainModel.Messages.Queries;
using Watchman.Web.Areas.Statistics.Models.Dtos;

namespace Watchman.Web.Areas.Statistics.Controllers
{
    public class StatisticsController : BaseApiController
    {
        private readonly IQueryBus queryBus;

        public StatisticsController(IQueryBus queryBus)
        {
            this.queryBus = queryBus;
        }

        //[HttpGet]
        //public async Task<IEnumerable<PeriodStatisticDto>> GetMessagesStatisticsPerDay()
        //{
        //    var query = new GetMessagesStatisticsQuery(Period.Day);
        //    var result = await this.queryBus.ExecuteAsync(query);
        //    return result.PeriodStatistics.Select(x => new PeriodStatisticDto(x));
        //}
    }
}
