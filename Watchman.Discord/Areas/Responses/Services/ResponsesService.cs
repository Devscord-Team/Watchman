using System;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Responses;
using Watchman.DomainModel.Responses.Commands;
using Watchman.DomainModel.Responses.Queries;

namespace Watchman.Discord.Areas.Responses.Services
{
    public class ResponsesService
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;

        public ResponsesService(IQueryBus queryBus, ICommandBus commandBus)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
        }

        public async Task<Response> GetResponseByOnEvent(string onEvent, ulong serverId = 0)
        {
            var query = new GetResponseQuery(onEvent, serverId);
            var queryResult = await this._queryBus.ExecuteAsync(query);
            var response = queryResult.Response;
            return response;
        }

        public async Task AddResponse(string onEvent, string message, ulong serverId = 0)
        {
            var query = new GetResponseQuery(onEvent);
            var response = (await this._queryBus.ExecuteAsync(query)).Response;
            var addResponse = new Response(onEvent, message, serverId, response.AvailableVariables);
            var command = new AddResponseCommand(addResponse);
            await this._commandBus.ExecuteAsync(command);
        }

        public async Task UpdateResponse(Guid id, string message)
        {
            var command = new UpdateResponseCommand(id, message);
            await this._commandBus.ExecuteAsync(command);
        }

        public async Task RemoveResponse(string onEvent, ulong serverId)
        {
            var command = new RemoveResponseCommand(onEvent, serverId);
            await this._commandBus.ExecuteAsync(command);
        }
    }
}
