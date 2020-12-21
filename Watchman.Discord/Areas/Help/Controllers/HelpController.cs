using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Help.BotCommands;
using System.Threading.Tasks;
using Watchman.Discord.Areas.Help.Services;
using Watchman.DomainModel.Help.Queries;
using Watchman.DomainModel.Help;
using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using System;

namespace Watchman.Discord.Areas.Help.Controllers
{
    public class HelpController : IController
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly HelpMessageGeneratorService _helpMessageGenerator;
        private readonly ResponsesService _responsesService;
        private readonly HelpService _helpService;

        public HelpController(MessagesServiceFactory messagesServiceFactory, HelpMessageGeneratorService messageGeneratorService,
            ResponsesService responsesService, HelpService helpService)
        {
            this._messagesServiceFactory = messagesServiceFactory;
            this._helpMessageGenerator = messageGeneratorService;
            this._responsesService = responsesService;
            this._helpService = helpService;
        }

        public Task PrintHelp(HelpCommand command, Contexts contexts)
        {
            var helpInformations = this._helpService.GetHelpInformations(contexts);
            return string.IsNullOrEmpty(command.Command)
                ? _helpService.PrintHelpForAllCommands(command.Json, contexts, helpInformations)
                : this._helpService.PrintHelpForOneCommand(command.Command, contexts, helpInformations);
        }
    }
}
