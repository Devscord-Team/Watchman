using System.Collections.Generic;
using Watchman.Cqrs;
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
        private readonly IQueryBus _queryBus;

        public ResponsesGetterService(IQueryBus queryBus)
        {
            this._queryBus = queryBus;
        }

        public IEnumerable<Response> GetResponsesFromBase()
        {
            var query = new GetResponsesQuery();
            var responsesInBase = this._queryBus.Execute(query).Responses;
            return responsesInBase;
        }

        public IEnumerable<Response> GetResponsesFromResources()
        {
            return default;
        }
    }
}
