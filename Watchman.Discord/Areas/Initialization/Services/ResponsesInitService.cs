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
            var defaultResponses = this._responsesGetterService.GetResponsesFromResources();
            var responsesInBase = this._responsesGetterService.GetResponsesFromBase().Select(baseResponse =>
            {
                // Update every response in the DB with availableVariables from defaultResponses
                var newAvailableVariables = defaultResponses
                    .FirstOrDefault(defaultResponse => defaultResponse.OnEvent == baseResponse.OnEvent);
                if (newAvailableVariables?.AvailableVariables != null)
                {
                    baseResponse.UpdateAvailableVariables(newAvailableVariables.AvailableVariables);
                }
                return baseResponse;
            });
            var responsesToUpdate = defaultResponses.Concat(responsesInBase).ToList();

            await this.UpdateResponses(responsesToUpdate);
        }

        private async Task UpdateResponses(IReadOnlyCollection<Response> responsesToUpdate)
        {
            if (!responsesToUpdate.Any())
            {
                Log.Information("No new responses");
                return;
            }

            var command = new UpdateResponsesCommand(responsesToUpdate);
            await this._commandBus.ExecuteAsync(command);
            Log.Information("Responses initialized");
        }
    }
}
