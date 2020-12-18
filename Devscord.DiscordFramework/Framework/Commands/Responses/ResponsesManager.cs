using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System;
using System.Collections.Generic;

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

        public static string SpamAlertRecognized(this ResponsesService responsesService, Contexts contexts)
        {
            return responsesService.ProcessResponse("SpamAlertRecognized", contexts);
        }

        public static string NewUserArrived(this ResponsesService responsesService, Contexts contexts)
        {
            return responsesService.ProcessResponse("NewUserArrived",
                new KeyValuePair<string, string>("user", contexts.User.Mention),
                new KeyValuePair<string, string>("server", contexts.Server.Name));
        }

        public static string UserIsNotAdmin(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("UserIsNotAdmin");
        }

        public static string NotAdminPermissions(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("NotAdminPermissions");
        }

        public static string UserDidntMentionAnyUser(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("UserDidntMentionAnyUser");
        }

        public static string UserNotFound(this ResponsesService responsesService, string user)
        {
            return responsesService.ProcessResponse("UserNotFound",
                new KeyValuePair<string, string>("user", user));
        }

        public static string RoleNotFound(this ResponsesService responsesService, string role)
        {
            return responsesService.ProcessResponse("RoleNotFound",
                new KeyValuePair<string, string>("role", role));
        }

        public static string MutedUser(this ResponsesService responsesService, UserContext user, DateTime timeEnd)
        {
            return responsesService.ProcessResponse("MutedUser",
                new KeyValuePair<string, string>("user", user.Name), //todo: change to GetMention when message will be send as Embed
                new KeyValuePair<string, string>("timeEnd", timeEnd.ToLocalTimeString()));
        }

        public static string UnmutedUser(this ResponsesService responsesService, UserContext user)
        {
            return responsesService.ProcessResponse("UnmutedUser",
                new KeyValuePair<string, string>("user", user.Name));
        }

        public static string UnmutedUserForUser(this ResponsesService responsesService, UserContext user, DiscordServerContext server)
        {
            return responsesService.ProcessResponse("UnmutedUserForUser",
                new KeyValuePair<string, string>("user", user.Name),
                new KeyValuePair<string, string>("server", server.Name));
        }

        public static string TimeCannotBeNegative(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("TimeCannotBeNegative");
        }

        public static string TimeIsTooBig(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("TimeIsTooBig");
        }

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

        public static string SentByDmMessagesOfAskedUser(this ResponsesService responsesService, int messagesCount, UserContext user)
        {
            return responsesService.ProcessResponse("SentByDmMessagesOfAskedUser",
                new KeyValuePair<string, string>("messagesCount", messagesCount.ToString()),
                new KeyValuePair<string, string>("user", user.Name));
        }

        public static string NumberOfMessagesIsHuge(this ResponsesService responsesService, int messagesCount)
        {
            return responsesService.ProcessResponse("NumberOfMessagesIsHuge",
                new KeyValuePair<string, string>("messagesCount", messagesCount.ToString()));
        }

        public static string ServerDoesntHaveAnySafeRoles(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ServerDoesntHaveAnySafeRoles");
        }

        public static string AvailableSafeRoles(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("AvailableSafeRoles");
        }

        public static string AvailableSafeRolesDescription(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("AvailableSafeRolesDescription");
        }

        public static string NotEnoughArguments(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("NotEnoughArguments");
        }

        public static string RoleSettingsChanged(this ResponsesService responsesService, string role)
        {
            return responsesService.ProcessResponse("RoleSettingsChanged",
                new KeyValuePair<string, string>("role", role));
        }

        public static string TimeNotSpecified(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("TimeNotSpecified");
        }

        public static string ArgumentsDuplicated(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ArgumentsDuplicated");
        }

        public static string RoleIsSafeAlready(this ResponsesService responsesService, string role)
        {
            return responsesService.ProcessResponse("RoleIsSafeAlready",
                new KeyValuePair<string, string>("role", role));
        }

        public static string RoleIsUnsafeAlready(this ResponsesService responsesService, string role)
        {
            return responsesService.ProcessResponse("RoleIsUnsafeAlready",
                new KeyValuePair<string, string>("role", role));
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

        public static string ResponseTheSameAsDefault(this ResponsesService responsesService, Contexts contexts, string onEvent)
        {
            return responsesService.ProcessResponse("ResponseTheSameAsDefault", contexts,
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

        public static string InvalidArguments(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("InvalidArguments");
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

        public static string UserHasBeenWarned(this ResponsesService responsesService, string grantor, string receiver, string reason)
        {
            return responsesService.ProcessResponse("UserHasBeenWarned",
                new KeyValuePair<string, string>("grantor", grantor),
                new KeyValuePair<string, string>("receiver", receiver),
                new KeyValuePair<string, string>("reason", reason));
        }

        public static string UserWasntMuted(this ResponsesService responsesService, UserContext user)
        {
            return responsesService.ProcessResponse("UserWasntMuted",
                new KeyValuePair<string, string>("user", user.Name));
        }

        public static string RoleAlreadyIsTrusted(this ResponsesService responsesService, string roleName)
        {
            return responsesService.ProcessResponse("RoleAlreadyIsTrusted",
                new KeyValuePair<string, string>("role", roleName));
        }

        public static string RoleAlreadyIsUntrusted(this ResponsesService responsesService, string roleName)
        {
            return responsesService.ProcessResponse("RoleAlreadyIsUntrusted",
                new KeyValuePair<string, string>("role", roleName));
        }

        public static string RoleSetAsTrusted(this ResponsesService responsesService, string roleName)
        {
            return responsesService.ProcessResponse("RoleSetAsTrusted",
                new KeyValuePair<string, string>("role", roleName));
        }
        
        public static string RoleSetAsUntrusted(this ResponsesService responsesService, string roleName)
        {
            return responsesService.ProcessResponse("RoleSetAsUntrusted",
                new KeyValuePair<string, string>("role", roleName));
        }

        public static string ComplaintsChannelAlreadyExists(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ComplaintsChannelAlreadyExists");
        }

        public static string ComplaintsChannelHasBeenCreated(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ComplaintsChannelHasBeenCreated");
        }

        public static string ServerDoesntHaveAnyTrustedRole(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ServerDoesntHaveAnyTrustedRole");
        }

        public static string MoreThanOneRegexHasBeenMatched(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("MoreThanOneRegexHasBeenMatched");
        }

        public static string MutedUsersListSent(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("MutedUsersListSent");
        }
      
        public static string TryToGoogleIt(this ResponsesService responsesService, string link)
        {
            return responsesService.ProcessResponse("TryToGoogleIt",
                new KeyValuePair<string, string>("link", link));
        }

        public static string ThereAreNoMutedUsers(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ThereAreNoMutedUsers");
        }

        public static string NoDefaultDescription(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("NoDefaultDescription");
        }

        public static string AvailableCommands(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("AvailableCommands");
        }

        public static string HereYouCanFindAvailableCommandsWithDescriptions(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("HereYouCanFindAvailableCommandsWithDescriptions");
        }

        public static string HowToUseCommand(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("HowToUseCommand");
        }

        public static string Example(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("Example");
        }

        public static string Type(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("Type");
        }

        public static string Parameters(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("Parameters");
        }

        public static string ExampleChannelMention(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ExampleChannelMention");
        }

        public static string ExampleUserMention(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ExampleUserMention");
        }

        public static string ExampleList(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ExampleList");
        }

        public static string ExampleSingleWord(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ExampleSingleWord");
        }

        public static string ExampleText(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ExampleText");
        }

        public static string Roles(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("Roles");
        }

        public static string InviteMe(this ResponsesService responsesService, Contexts contexts, string inviteBotUrl)
        {
            return responsesService.ProcessResponse("InviteMe", contexts,
                new KeyValuePair<string, string>("inviteBotUrl", inviteBotUrl));
        }
    }
}
