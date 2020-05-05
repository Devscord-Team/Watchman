using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Messages;
using Watchman.DomainModel.Messages.Queries;
using Watchman.DomainModel.Responses.Commands;
using Watchman.DomainModel.Responses.Queries;
using Watchman.Web.Server.Areas.Commons.Integration;
using Watchman.Web.Server.Areas.Messages.Models.Dtos;
using Watchman.Web.Server.Areas.Responses.Models.Dtos;

namespace Watchman.Web.Server.Areas.Messages.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IQueryBus _queryBus;
        private const int LimitForQuery = 100;
        public MessagesController(IQueryBus queryBus)
        {
            this._queryBus = queryBus;
        }

        [HttpPost]
        public async Task<IEnumerable<MessageDto>> GetMessages([FromBody]GetMessagesRequest request)
        {
            var query = new GetMessagesQuery(request.GetGuildId, request.GetChannelId, request.GetUserId);
            var responseResult = await _queryBus.ExecuteAsync(query);
            var messagesDto = responseResult.Messages.Take(LimitForQuery).Select(x => new MessageDto(x));
            return messagesDto;
        }
    }
}