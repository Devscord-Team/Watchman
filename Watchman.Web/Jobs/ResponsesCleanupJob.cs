using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Services.Models;
using Watchman.Discord.Areas.Responses.Services;
using Watchman.DomainModel.Responses;

namespace Watchman.Web.Jobs
{
    public class ResponsesCleanupJob : IHangfireJob
    {
        public RefreshFrequent Frequency => RefreshFrequent.Daily;
        public bool RunOnStart => false;

        private readonly ResponsesGetterService _responsesGetterService;
        private readonly CustomResponsesService _responsesService;

        public ResponsesCleanupJob(ResponsesGetterService responsesGetterService, CustomResponsesService responsesService)
        {
            this._responsesGetterService = responsesGetterService;
            this._responsesService = responsesService;
        }

        public async Task Do() 
            => await this.CleanDuplicatedResponses();

        private async Task CleanDuplicatedResponses()
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
