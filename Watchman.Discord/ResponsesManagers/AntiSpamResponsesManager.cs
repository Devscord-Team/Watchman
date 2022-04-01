using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Watchman.Discord.ResponsesManagers
{
    public static class AntiSpamResponsesManager
    {
        public static string SpamAlertRecognized(this IResponsesService responsesService, Contexts contexts)
        {
            return responsesService.ProcessResponse("SpamAlertRecognized", contexts);
        }
    }
}
