using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Responses.Queries;
using Watchman.Web.Areas.Responses.Models;

namespace Watchman.Web.Areas.Responses.Services
{
    public class ResponsesService
    {
        private readonly IQueryBus queryBus;
        private readonly ICommandBus commandBus;

        public ResponsesService(IQueryBus queryBus, ICommandBus commandBus)
        {
            this.queryBus = queryBus;
            this.commandBus = commandBus;
        }

        public IEnumerable<ResponseDto> GetResponses()
        {
            return this.queryBus.Execute(new GetResponsesQuery()).Responses
                .Select(x => new ResponseDto(x));
        }
    }
}
