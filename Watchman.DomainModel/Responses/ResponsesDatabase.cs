using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;
using Watchman.DomainModel.Responses.Queries;
using System.Linq;

namespace Watchman.DomainModel.Responses
{
    public class ResponsesDatabase
    {
        private readonly ulong _ServerID;
        private readonly IQueryBus _queryBus;

        public ResponsesDatabase(IQueryBus queryBus, ulong ServerID)
        {
            _queryBus = queryBus;
            _ServerID = ServerID;
        }

        public IEnumerable<Response> GetResponsesFromBase()
        {
            var query = new GetResponsesQuery();
            var responsesInBase = _queryBus.Execute(query).Responses;
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
                    return new Response(onEvent, message, _ServerID);
                })
                .ToList();

            return defaultResponses;
        }
    }
}
