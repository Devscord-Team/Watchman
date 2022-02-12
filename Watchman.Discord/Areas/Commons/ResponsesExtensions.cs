using Devscord.DiscordFramework.Commands.Responses;
using System.Linq;
using Watchman.Cqrs;
using Watchman.DomainModel.Responses.Queries;

namespace Watchman.Discord.Areas.Commons
{
    public static class ResponsesExtensions
    {
        public static ResponsesCachingService SetGetResponsesFromDatabase(this ResponsesCachingService service, IQueryBus queryBus)
        {
            service.GetResponsesFunc = serverId =>
            {
                var responsesInBase = queryBus.Execute(new GetResponsesQuery()).Responses.ToList();
                var serverResponses = responsesInBase.Where(x => x.ServerId == serverId).ToList();
                var defaultResponses = responsesInBase.Where(x => x.ServerId == 0).ToList();

                var overridedOnEvents = serverResponses.Select(x => x.OnEvent).ToList();

                var responsesNotOverridedByServer = defaultResponses.Where(x => !overridedOnEvents.Contains(x.OnEvent)).ToList();
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
