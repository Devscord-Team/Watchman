using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System;
using System.Collections.Generic;

namespace Devscord.DiscordFramework.Commands.Responses
{
    public static class FrameworkExceptionsResponsesManager
    {
        public static string ArgumentsDuplicated(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ArgumentsDuplicated");
        }
        public static string ComplaintsChannelAlreadyExists(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ComplaintsChannelAlreadyExists");
        }

        public static string InvalidArguments(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("InvalidArguments");
        }
        public static string MoreThanOneRegexHasBeenMatched(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("MoreThanOneRegexHasBeenMatched");
        }

        public static string NotAdminPermissions(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("NotAdminPermissions");
        }
        public static string NotEnoughArguments(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("NotEnoughArguments");
        }

        public static string RoleNotFound(this IResponsesService responsesService, string role)
        {
            return responsesService.ProcessResponse("RoleNotFound",
                new KeyValuePair<string, string>("role", role));
        }

        public static string TimeCannotBeNegative(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("TimeCannotBeNegative");
        }

        public static string TimeIsTooBig(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("TimeIsTooBig");
        }

        public static string TimeNotSpecified(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("TimeNotSpecified");
        }

        public static string UserNotFound(this IResponsesService responsesService, string user)
        {
            return responsesService.ProcessResponse("UserNotFound",
                new KeyValuePair<string, string>("user", user));
        }
    }
}
