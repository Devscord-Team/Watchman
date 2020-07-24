﻿using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Responses.Commands;
using Watchman.DomainModel.Responses.Queries;
using Watchman.Web.Areas.Responses.Models.Dtos;

namespace Watchman.Web.Areas.Responses.Controllers
{
    public class ResponsesController : BaseApiController
    {
        private readonly IQueryBus queryBus;
        private readonly ICommandBus commandBus;

        public ResponsesController(IQueryBus queryBus, ICommandBus commandBus)
        {
            this.queryBus = queryBus;
            this.commandBus = commandBus;
        }

        [HttpGet]
        public async Task<IEnumerable<ResponseDto>> GetResponses()
        {
            var query = new GetResponsesQuery();
            var responsesResult = await this.queryBus.ExecuteAsync(query);
            var responsesDto = responsesResult.Responses.Select(x => new ResponseDto(x));
            return responsesDto;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateResponse([FromBody] ResponseDto request)
        {
            var command = new UpdateResponseCommand(request.Id, request.Message);
            await this.commandBus.ExecuteAsync(command);
            return this.Ok();
        }
    }
}
