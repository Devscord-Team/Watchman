using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Watchman.Cqrs;
using Watchman.DomainModel.Help.Queries;
using Watchman.Web.Areas.Helps.Models.Dtos;

namespace Watchman.Web.Areas.Helps.Controllers
{
    public class HelpsController : BaseApiController
    {
        private readonly IQueryBus _queryBus;

        public HelpsController(IQueryBus queryBus)
        {
            this._queryBus = queryBus;
        }

        [HttpGet]
        public IEnumerable<HelpInformationDto> GetHelpInformations(ulong serverId = 0)
        {
            var query = new GetHelpInformationQuery(serverId);
            var responses = this._queryBus.Execute(query).HelpInformations;
            var helpInformation = responses.Select(x => new HelpInformationDto(x));
            return helpInformation;
        }
    }
}
