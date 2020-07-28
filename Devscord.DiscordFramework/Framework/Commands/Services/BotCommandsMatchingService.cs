using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;

namespace Devscord.DiscordFramework.Framework.Commands.Services
{
    public class BotCommandsMatchingService
    {
        public bool IsMatchedWithCommand(DiscordRequest request, BotCommandTemplate template)
        {
            if (!request.IsCommandForBot)
            {
                return false;
            }
            return request.Name.ToLowerInvariant() == template.NormalizedCommandName;
        }
    }
}
