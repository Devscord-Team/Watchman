using System;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using System.Linq;
using Watchman.Cqrs;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Watchman.Discord.Areas.Help.Services;

namespace Watchman.Discord.Areas.Help.Controllers
{
    public class HelpController : IController
    {
        private readonly IQueryBus _queryBus;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly HelpService _helpService;

        public HelpController(IQueryBus queryBus, MessagesServiceFactory messagesServiceFactory, HelpService helpService)
        {
            this._queryBus = queryBus;
            this._messagesServiceFactory = messagesServiceFactory;
            _helpService = helpService;
        }

        [DiscordCommand("help")]
        public void PrintHelp(DiscordRequest request, Contexts contexts)
        {
            var messagesService = _messagesServiceFactory.Create(contexts);

            if (!request.Arguments.Any(arg => arg.Values.Any(v => v == "json")))
            {
                var helpMessage = this._helpService.GenerateHelp(contexts);
                messagesService.SendResponse(x => x.PrintHelp(helpMessage), contexts);
                return;
            }

            var helpMessages = this._helpService.GenerateJsonHelp(contexts);
            helpMessages.ToList().ForEach(x => messagesService.SendMessage(x));
        }
    }
}
