using System.Collections.Generic;

namespace Devscord.DiscordFramework.Framework.Commands.Responses
{
    public static class ResponsesManager
    {
        public static string RoleAddedToUser(this ResponsesService responsesService, string user, string role)
        {
            return responsesService.ProcessResponse("RoleAddedToUser",
                new KeyValuePair<string, string>("user", user),
                new KeyValuePair<string, string>("role", role));
        }

        public static string RoleRemovedFromUser(this ResponsesService responsesService, string user, string role)
        {
            return responsesService.ProcessResponse("RoleRemovedFromUser",
                new KeyValuePair<string, string>("user", user),
                new KeyValuePair<string, string>("role", role));
        }

        public static string SpamAlertRecognized(this ResponsesService responsesService, string user, string channel)
        {
            return responsesService.ProcessResponse("SpamAlertRecognized",
                new KeyValuePair<string, string>("user", user),
                new KeyValuePair<string, string>("channel", channel));
        }
        
        public static string NewUserArrived(this ResponsesService responsesService, string user, string channel)
        {
            return responsesService.ProcessResponse("NewUserArrived",
                new KeyValuePair<string, string>("user", user),
                new KeyValuePair<string, string>("channel", channel));
        }
    }
}
