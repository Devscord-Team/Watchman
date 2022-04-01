using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System;
using System.Collections.Generic;

namespace Devscord.DiscordFramework.Commands.Responses
{
    public static class ResponsesManager //todo use configuration instead
    {
        public static string RoleAddedToUser(this IResponsesService responsesService, Contexts contexts, string role)
        {
            return responsesService.ProcessResponse("RoleAddedToUser", contexts,
                new KeyValuePair<string, string>("role", role));
        }

        public static string RoleRemovedFromUser(this IResponsesService responsesService, Contexts contexts, string role)
        {
            return responsesService.ProcessResponse("RoleRemovedFromUser", contexts,
                new KeyValuePair<string, string>("role", role));
        }

        public static string RoleNotFoundInUser(this IResponsesService responsesService, Contexts contexts, string role)
        {
            return responsesService.ProcessResponse("RoleNotFoundInUser", contexts,
                new KeyValuePair<string, string>("role", role));
        }

        public static string RoleNotFoundOrIsNotSafe(this IResponsesService responsesService, Contexts contexts, string role)
        {
            return responsesService.ProcessResponse("RoleNotFoundOrIsNotSafe", contexts,
                new KeyValuePair<string, string>("role", role));
        }

        public static string RoleIsInUserAlready(this IResponsesService responsesService, Contexts contexts, string role)
        {
            return responsesService.ProcessResponse("RoleIsInUserAlready", contexts,
                new KeyValuePair<string, string>("role", role));
        }

        public static string SpamAlertRecognized(this IResponsesService responsesService, Contexts contexts)
        {
            return responsesService.ProcessResponse("SpamAlertRecognized", contexts);
        }

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

        public static string MutedUser(this IResponsesService responsesService, UserContext user, DateTime timeEnd)
        {
            return responsesService.ProcessResponse("MutedUser",
                new KeyValuePair<string, string>("user", user.Name), //todo: change to GetMention when message will be send as Embed
                new KeyValuePair<string, string>("timeEnd", timeEnd.ToLocalTimeString()));
        }

        public static string UnmutedUser(this IResponsesService responsesService, UserContext user)
        {
            return responsesService.ProcessResponse("UnmutedUser",
                new KeyValuePair<string, string>("user", user.Name));
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

        public static string InvalidArguments(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("InvalidArguments");
        }

        public static string AllRolesAddedToUser(this IResponsesService responsesService, Contexts contexts)
        {
            return responsesService.ProcessResponse("AllRolesAddedToUser",
                new KeyValuePair<string, string>("user", contexts.User.Name));
        }

        public static string AllRolesRemovedFromUser(this IResponsesService responsesService, Contexts contexts)
        {
            return responsesService.ProcessResponse("AllRolesRemovedFromUser",
                new KeyValuePair<string, string>("user", contexts.User.Name));
        }

        public static string AllRolesSettingsChanged(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("AllRolesSettingsChanged");
        }

        public static string UserHasBeenWarned(this IResponsesService responsesService, string grantor, string receiver, string reason)
        {
            return responsesService.ProcessResponse("UserHasBeenWarned",
                new KeyValuePair<string, string>("grantor", grantor),
                new KeyValuePair<string, string>("receiver", receiver),
                new KeyValuePair<string, string>("reason", reason));
        }

        public static string UserWasntMuted(this IResponsesService responsesService, UserContext user)
        {
            return responsesService.ProcessResponse("UserWasntMuted",
                new KeyValuePair<string, string>("user", user.Name));
        }

        public static string RoleAlreadyIsTrusted(this IResponsesService responsesService, string roleName)
        {
            return responsesService.ProcessResponse("RoleAlreadyIsTrusted",
                new KeyValuePair<string, string>("role", roleName));
        }

        public static string RoleAlreadyIsUntrusted(this IResponsesService responsesService, string roleName)
        {
            return responsesService.ProcessResponse("RoleAlreadyIsUntrusted",
                new KeyValuePair<string, string>("role", roleName));
        }

        public static string RoleSetAsTrusted(this IResponsesService responsesService, string roleName)
        {
            return responsesService.ProcessResponse("RoleSetAsTrusted",
                new KeyValuePair<string, string>("role", roleName));
        }
        
        public static string RoleSetAsUntrusted(this IResponsesService responsesService, string roleName)
        {
            return responsesService.ProcessResponse("RoleSetAsUntrusted",
                new KeyValuePair<string, string>("role", roleName));
        }

        public static string ComplaintsChannelAlreadyExists(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ComplaintsChannelAlreadyExists");
        }

        public static string ComplaintsChannelHasBeenCreated(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ComplaintsChannelHasBeenCreated");
        }

        public static string ServerDoesntHaveAnyTrustedRole(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ServerDoesntHaveAnyTrustedRole");
        }

        public static string MoreThanOneRegexHasBeenMatched(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("MoreThanOneRegexHasBeenMatched");
        }

        public static string MutedUsersListSent(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("MutedUsersListSent");
        }
      
        public static string TryToGoogleIt(this IResponsesService responsesService, string link)
        {
            return responsesService.ProcessResponse("TryToGoogleIt",
                new KeyValuePair<string, string>("link", link));
        }

        public static string ThereAreNoMutedUsers(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ThereAreNoMutedUsers");
        }

        public static string NoDefaultDescription(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("NoDefaultDescription");
        }

        public static string AvailableCommands(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("AvailableCommands");
        }

        public static string HereYouCanFindAvailableCommandsWithDescriptions(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("HereYouCanFindAvailableCommandsWithDescriptions");
        }

        public static string HowToUseCommand(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("HowToUseCommand");
        }

        public static string Example(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("Example");
        }

        public static string Type(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("Type");
        }

        public static string Parameters(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("Parameters");
        }

        public static string ExampleChannelMention(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ExampleChannelMention");
        }

        public static string ExampleUserMention(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ExampleUserMention");
        }

        public static string ExampleList(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ExampleList");
        }

        public static string ExampleSingleWord(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ExampleSingleWord");
        }

        public static string ExampleText(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ExampleText");
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
