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
        private readonly IQueryBus _queryBus;
        private readonly ResponsesService _responsesService;

        public HelpController(MessagesServiceFactory messagesServiceFactory, HelpMessageGeneratorService messageGeneratorService, IQueryBus queryBus, ResponsesService responsesService)
        {
            this._messagesServiceFactory = messagesServiceFactory;
            this._helpMessageGenerator = messageGeneratorService;
            this._queryBus = queryBus;
            this._responsesService = responsesService;
        }

        public Task PrintHelp(HelpCommand command, Contexts contexts)
        {
            var helpInformations = this._queryBus.Execute(new GetHelpInformationQuery(contexts.Server.Id)).HelpInformations;
            return string.IsNullOrEmpty(command.Command) 
                ? this.PrintHelpForAllCommands(command, contexts, helpInformations) 
                : this.PrintHelpForOneCommand(command, contexts, helpInformations);
        }

        private Task PrintHelpForAllCommands(HelpCommand command, Contexts contexts, IEnumerable<HelpInformation> helpInformations)
        {
            var messagesService = this._messagesServiceFactory.Create(contexts);
            if (command.Json)
            {
                var helpMessage = this._helpMessageGenerator.GenerateJsonHelp(helpInformations);
                return messagesService.SendMessage(helpMessage, MessageType.Json);
            }
            var availableCommandsResponse = this._responsesService.GetResponse(contexts.Server.Id, x => x.AvailableCommands());
            var commandsDescriptionsResponses = this._responsesService.GetResponse(contexts.Server.Id, x => x.HereYouCanFindAvailableCommandsWithDescriptions());
            return messagesService.SendEmbedMessage(title: availableCommandsResponse, description: commandsDescriptionsResponses,
                this._helpMessageGenerator.MapHelpForAllCommandsToEmbed(helpInformations, contexts.Server));
        }

        private Task PrintHelpForOneCommand(HelpCommand command, Contexts contexts, IEnumerable<HelpInformation> helpInformations)
        {
            var helpInformation = helpInformations.FirstOrDefault(x => this.NormalizeCommandName(x.CommandName) == this.NormalizeCommandName(command.Command));
            if (helpInformation == null)
            {
                return Task.CompletedTask;
            }
            var messagesService = this._messagesServiceFactory.Create(contexts);
            var helpMessage = this._helpMessageGenerator.MapHelpForOneCommandToEmbed(helpInformation, contexts.Server);
            var description = helpInformation.Descriptions
                .FirstOrDefault(x => x.Language == helpInformation.DefaultLanguage)?.Text;
            if (string.IsNullOrEmpty(description))
            {
                description = this._responsesService.GetResponse(contexts.Server.Id, x => x.NoDefaultDescription());
            }
            var howToUseCommand = this._responsesService.GetResponse(contexts.Server.Id, x => x.HowToUseCommand());
            return messagesService.SendEmbedMessage($"{howToUseCommand} {helpInformation.CommandName.Replace("Command", string.Empty)}", description, helpMessage);
        }

        private string NormalizeCommandName(string x)
        {
            return x.ToLowerInvariant().TrimStart('-').Replace("command", string.Empty);
        }
    }
}
