using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Responses.Queries;
using Watchman.Web.Server.Areas.Responses.Models.Dtos;

namespace Watchman.Web.Server.Areas.Responses.Controllers
{
    public class ResponsesController : BaseApiController
    {
        private readonly IQueryBus queryBus;

        public ResponsesController(IQueryBus queryBus)
        {
            this.queryBus = queryBus;
        }


        [HttpGet]
        public IEnumerable<ResponseDto> GetResponses()
        {
            var query = new GetResponsesQuery();
            var responses = queryBus.Execute(query).Responses;
            var responsesDto = responses.Select(x => new ResponseDto(x));
            return responsesDto;
        }
    }
}
