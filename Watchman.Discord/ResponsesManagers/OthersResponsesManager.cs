using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System;
using System.Collections.Generic;

namespace Watchman.Discord.Areas.Commons
{
    public static class OthersResponsesManager
    {
        public static string NewUserArrived(this IResponsesService responsesService, Contexts contexts)
        {
            return responsesService.ProcessResponse("NewUserArrived",
                new KeyValuePair<string, string>("user", contexts.User.Mention),
                new KeyValuePair<string, string>("server", contexts.Server.Name));
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

        public static string TimeCannotBeNegative(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("TimeCannotBeNegative");
        }

        public static string TimeIsTooBig(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("TimeIsTooBig");
        }

        public static string UserDoesntHaveAvatar(this IResponsesService responsesService, UserContext user)
        {
            return responsesService.ProcessResponse("UserDoesntHaveAvatar",
                new KeyValuePair<string, string>("user", user.Mention));
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

        public static string ServerDoesntHaveAnySafeRoles(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ServerDoesntHaveAnySafeRoles");
        }

        public static string AvailableSafeRoles(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("AvailableSafeRoles");
        }

        public static string AvailableSafeRolesDescription(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("AvailableSafeRolesDescription");
        }

        public static string NotEnoughArguments(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("NotEnoughArguments");
        }

        public static string RoleSettingsChanged(this IResponsesService responsesService, string role)
        {
            return responsesService.ProcessResponse("RoleSettingsChanged",
                new KeyValuePair<string, string>("role", role));
        }

        public static string TimeNotSpecified(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("TimeNotSpecified");
        }

        public static string ArgumentsDuplicated(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ArgumentsDuplicated");
        }

        public static string RoleIsSafeAlready(this IResponsesService responsesService, string role)
        {
            return responsesService.ProcessResponse("RoleIsSafeAlready",
                new KeyValuePair<string, string>("role", role));
        }

        public static string RoleIsUnsafeAlready(this IResponsesService responsesService, string role)
        {
            return responsesService.ProcessResponse("RoleIsUnsafeAlready",
                new KeyValuePair<string, string>("role", role));
        }

        public static string InvalidArguments(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("InvalidArguments");
        }

        public static string MoreThanOneRegexHasBeenMatched(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("MoreThanOneRegexHasBeenMatched");
        }

        public static string Roles(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("Roles");
        }

        public static string CustomCommandsHeader(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("CustomCommandsHeader");
        }
    }
}
