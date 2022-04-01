using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System;
using System.Collections.Generic;

namespace Devscord.DiscordFramework.Commands.Responses
{
    public static class DefaultResponsesManager
    {
        public static string ArgumentsDuplicated(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ArgumentsDuplicated");
        }

        public static string UserIsNotAdmin(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("UserIsNotAdmin");
        }

        public static string NotAdminPermissions(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("NotAdminPermissions");
        }

        public static string UserDidntMentionAnyUser(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("UserDidntMentionAnyUser");
        }

        public static string UserNotFound(this IResponsesService responsesService, string user)
        {
            return responsesService.ProcessResponse("UserNotFound",
                new KeyValuePair<string, string>("user", user));
        }

        public static string RoleNotFound(this IResponsesService responsesService, string role)
        {
            return responsesService.ProcessResponse("RoleNotFound",
                new KeyValuePair<string, string>("role", role));
        }

        public static string UnmutedUserForUser(this IResponsesService responsesService, UserContext user, DiscordServerContext server)
        {
            return responsesService.ProcessResponse("UnmutedUserForUser",
                new KeyValuePair<string, string>("user", user.Name),
                new KeyValuePair<string, string>("server", server.Name));
        }

        public static string TimeCannotBeNegative(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("TimeCannotBeNegative");
        }

        public static string TimeIsTooBig(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("TimeIsTooBig");
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

        public static string NotEnoughArguments(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("NotEnoughArguments");
        }

        public static string TimeNotSpecified(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("TimeNotSpecified");
        }

        public static string InvalidArguments(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("InvalidArguments");
        }

        public static string MoreThanOneRegexHasBeenMatched(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("MoreThanOneRegexHasBeenMatched");
        }
    }
}
