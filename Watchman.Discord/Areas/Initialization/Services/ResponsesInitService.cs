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
            var defaultResponses = this._responsesGetterService.GetResponsesFromResources().ToList();
            var responsesInBase = this._responsesGetterService.GetResponsesFromBase();
            var responsesToUpdate = new List<Response>();
            var responsesToRemove = new List<Response>();
            
            foreach (var baseResponse in responsesInBase)
            {
                // Check if there's a matching default response in resources
                var matchingDefaultResponse = defaultResponses.FirstOrDefault(defaultResponse => defaultResponse.OnEvent == baseResponse.OnEvent);
                if (matchingDefaultResponse != null)
                {
                    // there's a matching default response, update existing db response
                    baseResponse.UpdateAvailableVariables(matchingDefaultResponse.AvailableVariables);
                    baseResponse.SetMessage(matchingDefaultResponse.Message);
                    responsesToUpdate.Add(baseResponse);
                }
                else
                {
                    // there's no matching default response, remove the db one
                    responsesToRemove.Add(baseResponse);
                }
            }
            
            var responsesToAdd = defaultResponses
                .Where(def => responsesToUpdate.All(@base => @base.OnEvent != def.OnEvent));
            
            var command = new RemoveResponsesCommand(responsesToRemove);
            await this._commandBus.ExecuteAsync(command);
            await this.AddOrUpdateResponses(responsesToAdd.Concat(responsesToUpdate).ToList());
        }

        private async Task AddOrUpdateResponses(IReadOnlyCollection<Response> responsesToUpdate)
        {
            if (!responsesToUpdate.Any())
            {
                Log.Information("No new responses");
                return;
            }

            var command = new AddOrUpdateResponsesCommand(responsesToUpdate);
            await this._commandBus.ExecuteAsync(command);
            Log.Information("Responses initialized");
        }
    }
}
