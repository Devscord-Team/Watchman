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

        public MessagesController(IQueryBus queryBus)
        {
            this._queryBus = queryBus;
        }

        [HttpGet]
        public async Task<IEnumerable<MessageDto>> GetMessages(ulong serverId, ulong channelId,  ulong? userId)
        {
            var query = new GetMessagesQuery(serverId);
            var responseResult = await _queryBus.ExecuteAsync(query);
            IEnumerable<MessageDto> messagesDto;
            if (userId.HasValue)
            {
                messagesDto = responseResult.Messages.Where(x => x.Author.Id == userId && x.Channel.Id == channelId).Take(100).Select(x => new MessageDto(x));
            }
            else
            {
                messagesDto = responseResult.Messages.Where(x => x.Channel.Id == channelId).Take(100).Select(x => new MessageDto(x));
            }
            return messagesDto;
        }
    }
}