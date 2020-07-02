using Devscord.DiscordFramework.Middlewares.Contexts;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Devscord.DiscordFramework.Framework.Commands.Responses
{
    public static class ResponsesManager
    {
        public static string RoleAddedToUser(this ResponsesService responsesService, Contexts contexts, string role)
        {
            return responsesService.ProcessResponse("RoleAddedToUser", contexts,
                new KeyValuePair<string, string>("role", role));
        }

        public static string RoleRemovedFromUser(this ResponsesService responsesService, Contexts contexts, string role)
        {
            return responsesService.ProcessResponse("RoleRemovedFromUser", contexts,
                new KeyValuePair<string, string>("role", role));
        }

        public static string RoleNotFoundInUser(this ResponsesService responsesService, Contexts contexts, string role)
        {
            return responsesService.ProcessResponse("RoleNotFoundInUser", contexts,
                new KeyValuePair<string, string>("role", role));
        }

        public static string RoleNotFoundOrIsNotSafe(this ResponsesService responsesService, Contexts contexts, string role)
        {
            return responsesService.ProcessResponse("RoleNotFoundOrIsNotSafe", contexts,
                new KeyValuePair<string, string>("role", role));
        }

        public static string RoleIsInUserAlready(this ResponsesService responsesService, Contexts contexts, string role)
        {
            return responsesService.ProcessResponse("RoleIsInUserAlready", contexts,
                new KeyValuePair<string, string>("role", role));
        }

        public static string SpamAlertRecognized(this ResponsesService responsesService, Contexts contexts) => responsesService.ProcessResponse("SpamAlertRecognized", contexts);

        public static string SpamAlertUserIsMutedForLong(this ResponsesService responsesService, Contexts contexts) => responsesService.ProcessResponse("SpamAlertUserIsMutedForLong", contexts);

        public static string SpamAlertUserIsMuted(this ResponsesService responsesService, Contexts contexts) => responsesService.ProcessResponse("SpamAlertUserIsMuted", contexts);

        public static string NewUserArrived(this ResponsesService responsesService, Contexts contexts)
        {
            return responsesService.ProcessResponse("NewUserArrived",
                new KeyValuePair<string, string>("user", contexts.User.Mention),
                new KeyValuePair<string, string>("server", contexts.Server.Name));
        }

        public static string PrintHelp(this ResponsesService responsesService, string help)
        {
            return responsesService.ProcessResponse("PrintHelp",
                new KeyValuePair<string, string>("help", help));
        }

<<<<<<< HEAD
        public static string UserIsNotAdmin(this ResponsesService responsesService) => responsesService.ProcessResponse("UserIsNotAdmin");
=======
        public static string NotAdminPermissions(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("NotAdminPermissions");
        }
>>>>>>> master

        public static string UserDidntMentionAnyUser(this ResponsesService responsesService) => responsesService.ProcessResponse("UserDidntMentionAnyUser");

        public static string UserNotFound(this ResponsesService responsesService, string userMention)
        {
            return responsesService.ProcessResponse("UserNotFound",
                new KeyValuePair<string, string>("user", userMention));
        }

        public static string RoleNotFound(this ResponsesService responsesService, string roleName)
        {
            return responsesService.ProcessResponse("RoleNotFound",
                new KeyValuePair<string, string>("role", roleName));
        }

        public static string MutedUser(this ResponsesService responsesService, UserContext mutedUser, DateTime timeEnd)
        {
            return responsesService.ProcessResponse("MutedUser",
                new KeyValuePair<string, string>("user", mutedUser.Name),
                new KeyValuePair<string, string>("timeEnd", timeEnd.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture)));
        }

        public static string UnmutedUser(this ResponsesService responsesService, UserContext unmutedUser)
        {
            return responsesService.ProcessResponse("UnmutedUser",
                new KeyValuePair<string, string>("user", unmutedUser.Name));
        }

        public static string UnmutedUserForUser(this ResponsesService responsesService, UserContext unmutedUser, DiscordServerContext server)
        {
            return responsesService.ProcessResponse("UnmutedUserForUser",
                new KeyValuePair<string, string>("user", unmutedUser.Name),
                new KeyValuePair<string, string>("server", server.Name));
        }

        public static string TimeCannotBeNegative(this ResponsesService responsesService) => responsesService.ProcessResponse("TimeCannotBeNegative");

        public static string TimeIsTooBig(this ResponsesService responsesService) => responsesService.ProcessResponse("TimeIsTooBig");

<<<<<<< HEAD
        public static string ReadingHistoryDone(this ResponsesService responsesService) => responsesService.ProcessResponse("ReadingHistoryDone");

=======
>>>>>>> master
        public static string UserDoesntHaveAvatar(this ResponsesService responsesService, UserContext user)
        {
            return responsesService.ProcessResponse("UserDoesntHaveAvatar",
                new KeyValuePair<string, string>("user", user.Mention));
        }

        public static string UserDidntWriteAnyMessageInThisTime(this ResponsesService responsesService, UserContext user)
        {
            return responsesService.ProcessResponse("UserDidntWriteAnyMessageInThisTime",
                new KeyValuePair<string, string>("user", user.Name));
        }

        public static string SentByDmMessagesOfAskedUser(this ResponsesService responsesService, int numberOfMessages, UserContext user)
        {
            return responsesService.ProcessResponse("SentByDmMessagesOfAskedUser",
                new KeyValuePair<string, string>("messagesCount", numberOfMessages.ToString()),
                new KeyValuePair<string, string>("user", user.Name));
        }

        public static string NumberOfMessagesIsHuge(this ResponsesService responsesService, int numberOfMessages)
        {
            return responsesService.ProcessResponse("NumberOfMessagesIsHuge",
                new KeyValuePair<string, string>("messagesCount", numberOfMessages.ToString()));
        }

        public static string ServerDoesntHaveAnySafeRoles(this ResponsesService responsesService) => responsesService.ProcessResponse("ServerDoesntHaveAnySafeRoles");

        public static string AvailableSafeRoles(this ResponsesService responsesService, string rolesLines)
        {
            return responsesService.ProcessResponse("AvailableSafeRoles",
                new KeyValuePair<string, string>("roles", rolesLines));
        }

        public static string NotEnoughArguments(this ResponsesService responsesService) => responsesService.ProcessResponse("NotEnoughArguments");

        public static string RoleSettingsChanged(this ResponsesService responsesService, string roleName)
        {
            return responsesService.ProcessResponse("RoleSettingsChanged",
                new KeyValuePair<string, string>("role", roleName));
        }

<<<<<<< HEAD
        public static string TimeNotSpecified(this ResponsesService responsesService) => responsesService.ProcessResponse("TimeNotSpecified");
=======
        public static string TimeNotSpecified(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("TimeNotSpecified");
        }
      
        public static string ArgumentsDuplicated(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ArgumentsDuplicated");
        }
>>>>>>> master

        public static string RoleIsSafeAlready(this ResponsesService responsesService, string roleName)
        {
            return responsesService.ProcessResponse("RoleIsSafeAlready",
                new KeyValuePair<string, string>("role", roleName));
        }

        public static string RoleIsUnsafeAlready(this ResponsesService responsesService, string roleName)
        {
            return responsesService.ProcessResponse("RoleIsUnsafeAlready" ,
                new KeyValuePair<string, string>("role", roleName));
        }
      
        public static string ResponseAlreadyExists(this ResponsesService responsesService, Contexts contexts, string onEvent)
        {
            return responsesService.ProcessResponse("ResponseAlreadyExists", contexts,
                new KeyValuePair<string, string>("onEvent", onEvent));
        }

        public static string ResponseHasBeenAdded(this ResponsesService responsesService, Contexts contexts, string onEvent)
        {
            return responsesService.ProcessResponse("ResponseHasBeenAdded", contexts,
                new KeyValuePair<string, string>("onEvent", onEvent));
        }

        public static string ResponseNotFound(this ResponsesService responsesService, Contexts contexts, string onEvent)
        {
            return responsesService.ProcessResponse("ResponseNotFound", contexts,
                new KeyValuePair<string, string>("onEvent", onEvent));
        }

        public static string ResponseHasBeenUpdated(this ResponsesService responsesService, Contexts contexts, string onEvent, string oldMessage, string newMessage)
        {
            return responsesService.ProcessResponse("ResponseHasBeenUpdated", contexts,
                new KeyValuePair<string, string>("onEvent", onEvent),
                new KeyValuePair<string, string>("oldMessage", oldMessage),
                new KeyValuePair<string, string>("newMessage", newMessage));
        }

        public static string ResponseHasBeenRemoved(this ResponsesService responsesService, Contexts contexts, string onEvent)
        {
            return responsesService.ProcessResponse("ResponseHasBeenRemoved", contexts,
                new KeyValuePair<string, string>("onEvent", onEvent));
        }

        public static string InvalidArguments(this ResponsesService responsesService, string arguments)
        {
            return responsesService.ProcessResponse("InvalidArguments",
                new KeyValuePair<string, string>("arguments", arguments));
        }

        public static string AllRolesAddedToUser(this ResponsesService responsesService, Contexts contexts)
        {
            return responsesService.ProcessResponse("AllRolesAddedToUser",
                new KeyValuePair<string, string>("user", contexts.User.Name));
        }
        
        public static string AllRolesRemovedFromUser(this ResponsesService responsesService, Contexts contexts)
        {
            return responsesService.ProcessResponse("AllRolesRemovedFromUser",
                new KeyValuePair<string, string>("user", contexts.User.Name));
        }

        public static string AllRolesSettingsChanged(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("AllRolesSettingsChanged");
        }
    }
}
