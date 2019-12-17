using Devscord.DiscordFramework.Middlewares.Contexts;
using System.Collections.Generic;

namespace Devscord.DiscordFramework.Framework.Commands.Responses
{
    public static class ResponsesManager
    {
        public static string RoleAddedToUser(this ResponsesService responsesService, Contexts contexts, string role)
        {
            return responsesService.ProcessResponse("RoleAddedToUser", contexts, new KeyValuePair<string, string>("role", role));
        }

        public static string RoleRemovedFromUser(this ResponsesService responsesService, Contexts contexts, string role)
        {
            return responsesService.ProcessResponse("RoleRemovedFromUser", contexts, new KeyValuePair<string, string>("role", role));
        }        
        
        public static string RoleNotFoundInUser(this ResponsesService responsesService, Contexts contexts, string role)
        {
            return responsesService.ProcessResponse("RoleNotFoundInUser", contexts, new KeyValuePair<string, string>("role", role));
        }        
        
        public static string RoleNotFoundOrIsNotSafe(this ResponsesService responsesService, Contexts contexts, string role)
        {
            return responsesService.ProcessResponse("RoleNotFoundOrIsNotSafe", contexts, new KeyValuePair<string, string>("role", role));
        }        

        public static string RoleIsInUserAlready(this ResponsesService responsesService, Contexts contexts, string role)
        {
            return responsesService.ProcessResponse("RoleIsInUserAlready", contexts, new KeyValuePair<string, string>("role", role));
        }

        public static string SpamAlertRecognized(this ResponsesService responsesService, Contexts contexts)
        {
            return responsesService.ProcessResponse("SpamAlertRecognized", contexts);
        }        
        
        public static string SpamAlertUserIsMuted(this ResponsesService responsesService, Contexts contexts)
        {
            return responsesService.ProcessResponse("SpamAlertUserIsMuted", contexts);
        }
        
        public static string NewUserArrived(this ResponsesService responsesService, Contexts contexts)
        {
            return responsesService.ProcessResponse("NewUserArrived", contexts);
        }

        public static string CurrentVersion(this ResponsesService responsesService, Contexts contexts, string version)
        {
            return responsesService.ProcessResponse("CurrentVersion", contexts, new KeyValuePair<string, string>("version", version));
        }
    }
}
