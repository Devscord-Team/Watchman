using System.Collections.Generic;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Watchman.Common.Strings;
using Watchman.Cqrs;
using Watchman.DomainModel.Help.Queries;
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
            var helpMessage = request.Arguments.Any(arg => arg.Values.Any(v => v == "json"))
                ? this._helpService.GenerateJsonHelp(contexts)
                : this._helpService.GenerateHelp(contexts);


            var messagesService = _messagesServiceFactory.Create(contexts);
            messagesService.SendResponse(x => x.PrintHelp(helpMessage), contexts);
        }
    }
}
