using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System;
using System.Collections.Generic;

namespace Watchman.Discord.Areas.Commons
{
    public static class OthersResponsesManager
    {
        public static string UserIsNotAdmin(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("UserIsNotAdmin");
        }

        public static string UserDidntWriteAnyMessageInThisTime(this IResponsesService responsesService, UserContext user)
        {
            return responsesService.ProcessResponse("UserDidntWriteAnyMessageInThisTime",
                new KeyValuePair<string, string>("user", user.Name));
        }

        public static string SentByDmMessagesOfAskedUser(this IResponsesService responsesService, int messagesCount, UserContext user)
        {
            return responsesService.ProcessResponse("SentByDmMessagesOfAskedUser",
                new KeyValuePair<string, string>("messagesCount", messagesCount.ToString()),
                new KeyValuePair<string, string>("user", user.Name));
        }

        public static string NumberOfMessagesIsHuge(this IResponsesService responsesService, int messagesCount)
        {
            return responsesService.ProcessResponse("NumberOfMessagesIsHuge",
                new KeyValuePair<string, string>("messagesCount", messagesCount.ToString()));
        }

        public static string TimeNotSpecified(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("TimeNotSpecified");
        }
    }
}
