using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Responses.Queries;
using Watchman.Web.Areas.Responses.Models.Dtos;

namespace Watchman.Web.Areas.Responses.Controllers
{
    public class ResponsesController : Controller
    {
        private readonly IQueryBus queryBus;

        public ResponsesController(IQueryBus queryBus)
        {
            this.queryBus = queryBus;
        }

        [HttpGet]
        public GetResponsesResponse GetResponses(GetResponsesRequest request)
        {
            var query = new GetResponsesQuery();
            var responses = queryBus.Execute(query).Responses.Select(x => new ResponseDto(x));
            return new GetResponsesResponse { Responses = responses };
        }

        [HttpGet]
        public string Ping()
        {
            return "pong";
        }

    }
}
