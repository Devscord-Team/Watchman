using Devscord.DiscordFramework.Commands.Responses;
using System.Collections.Generic;
using System.Text;

namespace Watchman.Discord.ResponsesManagers
{
    public static class AdministrationResponsesManager
    {
        public static string ServerDoesntHaveAnyTrustedRole(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ServerDoesntHaveAnyTrustedRole");
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

        public static string ComplaintsChannelHasBeenCreated(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ComplaintsChannelHasBeenCreated");
        }

        public static string ComplaintsChannelAlreadyExists(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ComplaintsChannelAlreadyExists");
        }
    }
}
