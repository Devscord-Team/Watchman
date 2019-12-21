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
        public static MessagesService SetGetResponsesFromDatabase(this MessagesService service, IQueryBus queryBus)
        {
            service.SetGetResponsesFunc(x =>
            {
                var responsesInBase = queryBus.Execute(new GetResponsesQuery()).Responses;
                var mapped = responsesInBase.Select(x => new Devscord.DiscordFramework.Framework.Commands.Responses.Response
                {
                    OnEvent = x.OnEvent,
                    Message = x.Message
                });
                return mapped;
            });
            return service;
        }
    }
}
