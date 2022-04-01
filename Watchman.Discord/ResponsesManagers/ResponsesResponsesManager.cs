using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System.Collections.Generic;

namespace Watchman.Discord.ResponsesManagers
{
    public static class ResponsesResponsesManager
    {
        public static string ResponseAlreadyExists(this IResponsesService responsesService, Contexts contexts, string onEvent)
        {
            return responsesService.ProcessResponse("ResponseAlreadyExists", contexts,
                new KeyValuePair<string, string>("onEvent", onEvent));
        }

        public static string ResponseHasBeenAdded(this IResponsesService responsesService, Contexts contexts, string onEvent)
        {
            return responsesService.ProcessResponse("ResponseHasBeenAdded", contexts,
                new KeyValuePair<string, string>("onEvent", onEvent));
        }

        public static string ResponseTheSameAsDefault(this IResponsesService responsesService, Contexts contexts, string onEvent)
        {
            return responsesService.ProcessResponse("ResponseTheSameAsDefault", contexts,
                new KeyValuePair<string, string>("onEvent", onEvent));
        }

        public static string ResponseNotFound(this IResponsesService responsesService, Contexts contexts, string onEvent)
        {
            return responsesService.ProcessResponse("ResponseNotFound", contexts,
                new KeyValuePair<string, string>("onEvent", onEvent));
        }

        public static string ResponseHasBeenUpdated(this IResponsesService responsesService, Contexts contexts, string onEvent, string oldMessage, string newMessage)
        {
            return responsesService.ProcessResponse("ResponseHasBeenUpdated", contexts,
                new KeyValuePair<string, string>("onEvent", onEvent),
                new KeyValuePair<string, string>("oldMessage", oldMessage),
                new KeyValuePair<string, string>("newMessage", newMessage));
        }

        public static string ResponseHasBeenRemoved(this IResponsesService responsesService, Contexts contexts, string onEvent)
        {
            return responsesService.ProcessResponse("ResponseHasBeenRemoved", contexts,
                new KeyValuePair<string, string>("onEvent", onEvent));
        }
    }
}
