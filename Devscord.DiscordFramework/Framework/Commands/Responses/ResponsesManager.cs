using System;
using Devscord.DiscordFramework.Middlewares.Contexts;
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

        public static string SpamAlertRecognized(this ResponsesService responsesService, Contexts contexts)
        {
            return responsesService.ProcessResponse("SpamAlertRecognized", contexts);
        }

        public static string SpamAlertUserIsMutedForLong(this ResponsesService responsesService, Contexts contexts)
        {
            return responsesService.ProcessResponse("SpamAlertUserIsMutedForLong", contexts);
        }

        public static string SpamAlertUserIsMuted(this ResponsesService responsesService, Contexts contexts)
        {
            return responsesService.ProcessResponse("SpamAlertUserIsMuted", contexts);
        }
        
        public static string NewUserArrived(this ResponsesService responsesService, Contexts contexts)
        {
            return responsesService.ProcessResponse("NewUserArrived", 
                new KeyValuePair<string, string>("user", contexts.User.Mention),
                new KeyValuePair<string, string>("server", contexts.Server.Name));
        }

        public static string CurrentVersion(this ResponsesService responsesService, Contexts contexts, string version)
        {
            return responsesService.ProcessResponse("CurrentVersion", contexts, 
                new KeyValuePair<string, string>("version", version));
        }

        public static string PrintHelp(this ResponsesService responsesService, string help)
        {
            return responsesService.ProcessResponse("PrintHelp",
                new KeyValuePair<string, string>("help", help));
        }

        public static string UserIsNotAdmin(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("UserIsNotAdmin");
        }

        public static string UserDidntMentionAnyUser(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("UserDidntMentionAnyUser");
        }  
        
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

        public static string TimeCannotBeNegative(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("TimeCannotBeNegative");
        }

        public static string TimeIsTooBig(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("TimeIsTooBig");
        }

        public static string ReadingHistoryDone(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ReadingHistoryDone");
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

        public static string ServerDoesntHaveAnySafeRoles(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ServerDoesntHaveAnySafeRoles");
        }

        public static string AvailableSafeRoles(this ResponsesService responsesService, string rolesLines)
        {
            return responsesService.ProcessResponse("AvailableSafeRoles",
                new KeyValuePair<string, string>("roles", rolesLines));
        }

        public static string TimeNotSpecified(this ResponsesService responsesService)
        {
            return responsesService.ProcessResponse("TimeNotSpecified");
        }
    }
}
