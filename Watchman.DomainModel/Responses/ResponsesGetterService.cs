using System.Collections.Generic;
using Watchman.Cqrs;
using Watchman.DomainModel.Responses.Areas.Administration;
using Watchman.DomainModel.Responses.Queries;
using Response = Watchman.DomainModel.Responses.Response;

namespace Watchman.Discord.Areas.Responses.Services
{
    public interface IResponsesGetterService
    {
        IEnumerable<Response> GetResponsesFromBase();
        IEnumerable<Response> GetResponsesFromResources();
    }

    public class ResponsesGetterService : IResponsesGetterService
    {
        private readonly IQueryBus queryBus;
        private readonly IResourcesResponsesService resourcesResponsesService;
        private readonly string[] areas = new[] //todo from configuration or auto
        {
            "FrameworkExceptions",
            "Administration",
            "AntiSpam",
            "Help",
            "Muting",
            "Responses",
            "UselessFeatures",
            "Users",
            "Warns"
        };

        public ResponsesGetterService(IQueryBus queryBus, IResourcesResponsesService resourcesResponsesService)
        {
            this.queryBus = queryBus;
            this.resourcesResponsesService = resourcesResponsesService;
        }

        public IEnumerable<Response> GetResponsesFromBase()
        {
            var query = new GetResponsesQuery();
            var responsesInBase = this.queryBus.Execute(query).Responses;
            return responsesInBase;
        }

        public IEnumerable<Response> GetResponsesFromResources()
            => this.resourcesResponsesService.GetResponses(this.areas);
    }
}
