using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System.Collections.Generic;

namespace Watchman.Discord.ResponsesManagers
{
    public static class UsersResponsesManager
    {
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

        public static string NewUserArrived(this IResponsesService responsesService, Contexts contexts)
        {
            return responsesService.ProcessResponse("NewUserArrived",
                new KeyValuePair<string, string>("user", contexts.User.Mention),
                new KeyValuePair<string, string>("server", contexts.Server.Name));
        }
    }
}
