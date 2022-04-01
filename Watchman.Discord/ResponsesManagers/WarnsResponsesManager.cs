using Devscord.DiscordFramework.Commands.Responses;
using System.Collections.Generic;

namespace Watchman.Discord.ResponsesManagers
{
    public static class WarnsResponsesManager
    {
        public static string UserHasBeenWarned(this IResponsesService responsesService, string grantor, string receiver, string reason)
        {
            return responsesService.ProcessResponse("UserHasBeenWarned",
                new KeyValuePair<string, string>("grantor", grantor),
                new KeyValuePair<string, string>("receiver", receiver),
                new KeyValuePair<string, string>("reason", reason));
        }
    }
}
