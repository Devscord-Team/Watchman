using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Watchman.Discord.Areas.Help.Services;
using Devscord.DiscordFramework.Commons;

namespace Watchman.Discord.Areas.Help.Controllers
{
    public class HelpController : IController
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly HelpMessageGeneratorService _helpMessageGenerator;

        public HelpController(MessagesServiceFactory messagesServiceFactory, HelpMessageGeneratorService messageGeneratorService)
        {
            this._messagesServiceFactory = messagesServiceFactory;
            _helpMessageGenerator = messageGeneratorService;
        }

        [DiscordCommand("help")]
        public void PrintHelp(DiscordRequest request, Contexts contexts)
        {
            var messagesService = _messagesServiceFactory.Create(contexts);

            if (request.HasArgument(null, "json"))
            {
                var helpMessage = this._helpMessageGenerator.GenerateJsonHelp(contexts);
                messagesService.SendMessage(helpMessage, MessageType.Json);
            }
            else
            {
                var helpMessage = this._helpMessageGenerator.GenerateHelp(contexts);
                messagesService.SendResponse(x => x.PrintHelp(helpMessage), contexts);
            }
        }
    }
}
