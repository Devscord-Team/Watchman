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

        public static string RoleAlreadyIsTrusted(this IResponsesService responsesService, string role)
        {
            return responsesService.ProcessResponse("RoleAlreadyIsTrusted",
                new KeyValuePair<string, string>("role", role));
        }
        public static string RoleAlreadyIsUntrusted(this IResponsesService responsesService, string role)
        {
            return responsesService.ProcessResponse("RoleAlreadyIsUntrusted",
                new KeyValuePair<string, string>("role", role));
        }

        public static string RoleSetAsTrusted(this IResponsesService responsesService, string role)
        {
            return responsesService.ProcessResponse("RoleSetAsTrusted",
                new KeyValuePair<string, string>("role", role));
        }

        public static string RoleSetAsUntrusted(this IResponsesService responsesService, string role)
        {
            return responsesService.ProcessResponse("RoleSetAsUntrusted",
                new KeyValuePair<string, string>("role", role));
        }

        public static string ComplaintsChannelHasBeenCreated(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ComplaintsChannelHasBeenCreated");
        }
    }
}
