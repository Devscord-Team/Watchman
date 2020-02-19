using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Help;
using Watchman.DomainModel.Help.Queries;

namespace Watchman.Web.Server.Areas.Helps.Controllers
{
    public class HelpsController
    {
        private readonly IQueryBus queryBus;

        public HelpsController(IQueryBus queryBus)
        {
            this.queryBus = queryBus;
        }

        [HttpGet]
        public IEnumerable<HelpInformation> GetHelpInformations(ulong serverId = 0)
        {
            var query = new GetHelpInformationQuery(serverId);
            var responses = queryBus.Execute(query).HelpInformations;
            return responses;
        }
    }
}
