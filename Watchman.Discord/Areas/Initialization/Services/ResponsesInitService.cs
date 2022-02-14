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
    public interface IResponsesInitService
    {
        Task InitNewResponsesFromResources();
    }

    public class ResponsesInitService : IResponsesInitService
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
                var matchingDefaultResponse = defaultResponses.FirstOrDefault(defaultResponse => defaultResponse.OnEvent == baseResponse.OnEvent);
                if (matchingDefaultResponse == null)
                {
                    responsesToRemove.Add(baseResponse);
                    continue;
                }
                baseResponse.UpdateAvailableVariables(matchingDefaultResponse.AvailableVariables);
                baseResponse.SetMessage(matchingDefaultResponse.Message);
                responsesToUpdate.Add(baseResponse);
            }

            var responsesToAdd = defaultResponses.Where(def => responsesToUpdate.All(@base => @base.OnEvent != def.OnEvent));

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
