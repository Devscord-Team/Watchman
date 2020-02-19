using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Watchman.Cqrs;
using Watchman.DomainModel.Help.Queries;
using Watchman.Web.Server.Areas.Helps.Models.Dtos;

namespace Watchman.Web.Server.Areas.Helps.Controllers
{
    public class HelpsController : BaseApiController
    {
        private readonly IQueryBus queryBus;

        public HelpsController(IQueryBus queryBus)
        {
            this.queryBus = queryBus;
        }

        [HttpGet]
        public IEnumerable<HelpInformationDto> GetHelpInformations(ulong serverId = 0)
        {
            var query = new GetHelpInformationQuery(serverId);
            var responses = queryBus.Execute(query).HelpInformations.Select(x => new HelpInformationDto(x));
            return responses;
        }
    }
}
