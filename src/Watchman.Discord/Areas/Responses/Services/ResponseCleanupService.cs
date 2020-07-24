using System.Linq;
using System.Threading.Tasks;
using Watchman.DomainModel.Responses;

namespace Watchman.Discord.Areas.Responses.Services
{
    public class ResponseCleanupService
    {
        private readonly ResponsesGetterService _responsesGetterService;
        private readonly ResponsesService _responsesService;

        public ResponseCleanupService(ResponsesGetterService responsesGetterService, ResponsesService responsesService)
        {
            _responsesGetterService = responsesGetterService;
            _responsesService = responsesService;
        }

        public async Task CleanDuplicatedResponses()
        {
            var allResponses = this._responsesGetterService.GetResponsesFromBase().ToList();
            var defaultResponses = allResponses.Where(resp => resp.ServerId == 0);
            var serverResponses = allResponses.Where(resp => resp.ServerId != 0);
            var responsesToDelete = serverResponses
                .Where(serverResponse => defaultResponses.Any(defaultResponse => CompareResponses(serverResponse, defaultResponse)));

            foreach (var response in responsesToDelete)
            {
                await this._responsesService.RemoveResponse(response.OnEvent, response.ServerId);
            }
        }

        private static bool CompareResponses(Response serverResponse, Response defaultResponse) 
        {
            return defaultResponse.Message == serverResponse.Message 
                && defaultResponse.OnEvent == serverResponse.OnEvent;
        }
    }
}