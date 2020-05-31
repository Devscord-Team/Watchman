using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Commands.Responses.Resources;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Responses.Services;
using Watchman.DomainModel.Responses;
using Watchman.DomainModel.Responses.Commands;
using Watchman.DomainModel.Responses.Queries;

namespace Watchman.Discord.Areas.Initialization.Services
{
    public class ResponsesInitService
    { 
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly ResponsesGetterService _responsesGetterService;

        public ResponsesInitService(IQueryBus queryBus, ICommandBus commandBus, ResponsesGetterService responsesGetterService)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _responsesGetterService = responsesGetterService;
        }

        public async Task InitNewResponsesFromResources()
        {
            var responsesInBase = _responsesGetterService.GetResponsesFromBase();
            var defaultResponses = _responsesGetterService.GetResponsesFromResources();
            var responsesToAdd = defaultResponses
                .Where(def => responsesInBase.All(@base => @base.OnEvent != def.OnEvent))
                .ToList();

            await AddNewResponses(responsesToAdd);
        }

        private async Task AddNewResponses(IReadOnlyCollection<Response> responsesToAdd)
        {
            if (!responsesToAdd.Any())
            {
                Log.Information("No new responses");
                return;
            }

            var command = new AddResponsesCommand(responsesToAdd);
            await _commandBus.ExecuteAsync(command);
            Log.Information("Responses initialized");
        }
    }
}
