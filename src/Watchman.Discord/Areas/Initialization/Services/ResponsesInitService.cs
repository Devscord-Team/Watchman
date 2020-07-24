using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Responses.Services;
using Watchman.DomainModel.Responses;
using Watchman.DomainModel.Responses.Commands;

namespace Watchman.Discord.Areas.Initialization.Services
{
    public class ResponsesInitService
    {
        private readonly ICommandBus _commandBus;
        private readonly ResponsesGetterService _responsesGetterService;

        public ResponsesInitService(ICommandBus commandBus, ResponsesGetterService responsesGetterService)
        {
            this._commandBus = commandBus;
            this._responsesGetterService = responsesGetterService;
        }

        public async Task InitNewResponsesFromResources()
        {
            var responsesInBase = this._responsesGetterService.GetResponsesFromBase();
            var defaultResponses = this._responsesGetterService.GetResponsesFromResources();
            var responsesToAdd = defaultResponses
                .Where(def => responsesInBase.All(@base => @base.OnEvent != def.OnEvent))
                .ToList();

            await this.AddNewResponses(responsesToAdd);
        }

        private async Task AddNewResponses(IReadOnlyCollection<Response> responsesToAdd)
        {
            if (!responsesToAdd.Any())
            {
                Log.Information("No new responses");
                return;
            }

            var command = new AddResponsesCommand(responsesToAdd);
            await this._commandBus.ExecuteAsync(command);
            Log.Information("Responses initialized");
        }
    }
}
