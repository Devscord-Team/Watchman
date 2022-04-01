using Devscord.DiscordFramework.Commands.Responses;
using System.Collections.Generic;

namespace Watchman.Discord.ResponsesManagers
{
    public static class UselessFeaturesResponsesManager
    {
        public static string TryToGoogleIt(this IResponsesService responsesService, string link)
        {
            return responsesService.ProcessResponse("TryToGoogleIt",
                new KeyValuePair<string, string>("link", link));
        }
    }
}
