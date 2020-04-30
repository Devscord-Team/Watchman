using Devscord.DiscordFramework.Framework.Commands.Responses;
using System.Linq;
using Watchman.Cqrs;
using Watchman.DomainModel.Responses.Queries;

namespace Watchman.Discord.Areas.Commons
{
    public static class ResponsesExtensions
    {
        public static ResponsesService SetGetResponsesFromDatabase(this ResponsesService service, IQueryBus queryBus)
        {
            service.GetResponsesFunc = contexts =>
            {
                var responsesInBase = queryBus.Execute(new GetResponsesQuery()).Responses;
                var serverResponses = responsesInBase.Where(x => x.ServerId == contexts.Server.Id);

                var overridedOnEvents = serverResponses.Select(x => x.OnEvent).ToList();

                var responsesNotOverridedByServer = responsesInBase.Where(x => !overridedOnEvents.Contains(x.OnEvent)).ToList();
                responsesNotOverridedByServer.AddRange(serverResponses);

                var mapped = responsesNotOverridedByServer.Select(x => new Response
                {
                    OnEvent = x.OnEvent,
                    Message = x.Message
                });
                return mapped;
            };
            return service;
        }
    }
}
