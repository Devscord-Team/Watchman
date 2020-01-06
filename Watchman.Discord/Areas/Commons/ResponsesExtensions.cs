using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Cqrs;
using Watchman.DomainModel.Responses.Queries;

namespace Watchman.Discord.Areas.Commons
{
    public static class ResponsesExtensions
    {
        public static ResponsesService SetGetResponsesFromDatabase(this ResponsesService service, IQueryBus queryBus)
        {
            service.GetResponsesFunc = x =>
            {
                var responsesInBase = queryBus.Execute(new GetResponsesQuery()).Responses;
                var mapped = responsesInBase.Select(x => new Response
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
