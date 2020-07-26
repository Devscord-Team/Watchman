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
            
            var responsesToUpdate = this._responsesGetterService.GetResponsesFromBase().Select(baseResponse =>
            {
                // Update every response in the DB with availableVariables from defaultResponses
                var matchingDefaultResponse = defaultResponses
                    .FirstOrDefault(defaultResponse => defaultResponse.OnEvent == baseResponse.OnEvent);
                if (matchingDefaultResponse != null)
                {
                    baseResponse.UpdateAvailableVariables(matchingDefaultResponse.AvailableVariables);
                    baseResponse.SetMessage(matchingDefaultResponse.Message);
                }
                return baseResponse;
            }).ToArray();
            
            var responsesToAdd = defaultResponses
                .Where(def => responsesToUpdate.All(@base => @base.OnEvent != def.OnEvent))
                .ToArray();

            await this.UpdateResponses(responsesToUpdate, responsesToAdd);
        }

        private async Task UpdateResponses(IReadOnlyCollection<Response> responsesToUpdate, IReadOnlyCollection<Response> responsesToAdd)
        {
            if (!responsesToUpdate.Any() && !responsesToAdd.Any())
            {
                Log.Information("No new responses");
                return;
            }

            var command = new UpdateResponsesCommand(responsesToUpdate, responsesToAdd);
            await this._commandBus.ExecuteAsync(command);
            Log.Information("Responses initialized");
        }
    }
}
