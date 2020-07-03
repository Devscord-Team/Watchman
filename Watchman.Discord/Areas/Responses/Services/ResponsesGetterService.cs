using System.Collections.Generic;
using System.Linq;
using Watchman.Cqrs;
using Watchman.DomainModel.Responses;
using Watchman.DomainModel.Responses.Queries;

namespace Watchman.Discord.Areas.Responses.Services
{
    public class ResponsesGetterService
    {
        private const int DEFAULT_SERVER_ID = 0;
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
            var defaultResponses = typeof(Devscord.DiscordFramework.Framework.Commands.Responses.Resources.Responses).GetProperties()
                .Where(x => x.PropertyType.Name == "String")
                .Select(prop =>
                {
                    var onEvent = prop.Name;
                    var message = prop.GetValue(prop)?.ToString();
                    return new Response(onEvent, message, DEFAULT_SERVER_ID);
                });
            return defaultResponses;
        }
    }
}
