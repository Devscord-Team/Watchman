using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Watchman.Cqrs;
using Watchman.DomainModel.Responses.Queries;
using Response = Watchman.DomainModel.Responses.Response;

namespace Watchman.Discord.Areas.Responses.Services
{
    public class ResponsesService
    {
        private readonly IQueryBus _queryBus;

        public ResponsesService(IQueryBus queryBus)
        {
            this._queryBus = queryBus;
        }

        public async Task<Response> GetResponseByOnEvent(string onEvent, ulong serverId = 0)
        {
            var query = new GetResponseQuery(onEvent, serverId);
            var queryResult = await this._queryBus.ExecuteAsync(query);
            var response = queryResult.Response;

            return response;
        }
    }
}
