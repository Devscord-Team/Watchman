﻿using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Help;
using Watchman.DomainModel.Help.Queries;
using Watchman.Discord.ResponsesManagers;

namespace Watchman.Discord.Areas.Help.Services
{
    public interface IHelpService
    {
        IEnumerable<HelpInformation> GetHelpInformations(Contexts contexts);
        Task PrintHelpForAllCommands(Contexts contexts, IEnumerable<HelpInformation> helpInformations, bool printAsJson);
        Task PrintHelpForOneCommand(string commandName, Contexts contexts, IEnumerable<HelpInformation> helpInformations, bool printAsJson = false);
    }

    public class HelpService : IHelpService
    {
        private readonly IMessagesServiceFactory _messagesServiceFactory;
        private readonly IHelpMessageGeneratorService _helpMessageGenerator;
        private readonly IResponsesService _responsesService;
        private readonly IQueryBus _queryBus;

        public HelpService(IMessagesServiceFactory messagesServiceFactory, IHelpMessageGeneratorService messageGeneratorService, IResponsesService responsesService, IQueryBus queryBus)
        {
            this._messagesServiceFactory =  messagesServiceFactory;
            this._helpMessageGenerator = messageGeneratorService;
            this._responsesService = responsesService;
            this._queryBus = queryBus;
        }

        public IEnumerable<HelpInformation> GetHelpInformations(Contexts contexts)
        {
            var helpInformations = this._queryBus.Execute(new GetHelpInformationsQuery(contexts.Server.Id)).HelpInformations;
            return helpInformations;
        }

        public Task PrintHelpForAllCommands(Contexts contexts, IEnumerable<HelpInformation> helpInformations, bool printAsJson)
        {
            var messagesService = this._messagesServiceFactory.Create(contexts);
            if (printAsJson)
            {
                var helpMessageJson = this._helpMessageGenerator.GenerateJsonHelp(helpInformations);
                return messagesService.SendMessage(helpMessageJson, MessageType.Json);
            }
            var availableCommandsResponse = this._responsesService.GetResponse(contexts.Server.Id, x => x.AvailableCommands());
            var commandsDescriptionsResponses = this._responsesService.GetResponse(contexts.Server.Id, x => x.HereYouCanFindAvailableCommandsWithDescriptions());
            return messagesService.SendEmbedMessage(title: availableCommandsResponse, description: commandsDescriptionsResponses,
                this._helpMessageGenerator.MapHelpForAllCommandsToEmbed(helpInformations, contexts.Server));
        }
        public Task PrintHelpForOneCommand(string commandName, Contexts contexts, IEnumerable<HelpInformation> helpInformations, bool IsJson = false)
        {
            var helpInformation = helpInformations.FirstOrDefault(x => this.NormalizeCommandName(x.CommandName) == this.NormalizeCommandName(commandName));
            if (helpInformation == null)
            {
                return Task.CompletedTask;
            }
            var messagesService = this._messagesServiceFactory.Create(contexts);
            if (IsJson)
            {
                var helpMessageJson = this._helpMessageGenerator.GenerateJsonHelpForOneCommand(helpInformation);
                return messagesService.SendMessage(helpMessageJson, MessageType.Json);
            }
            var helpMessage = this._helpMessageGenerator.MapHelpForOneCommandToEmbed(helpInformation, contexts.Server);
            var description = helpInformation.Descriptions
                .FirstOrDefault(x => x.Language == helpInformation.DefaultLanguage)?.Text;
            if (string.IsNullOrEmpty(description))
            {
                description = this._responsesService.GetResponse(contexts.Server.Id, x => x.NoDefaultDescription());
            }
            var howToUseCommand = this._responsesService.GetResponse(contexts.Server.Id, x => x.HowToUseCommand());
            return messagesService.SendEmbedMessage($"{howToUseCommand} {helpInformation.CommandName.Replace("Command", string.Empty)}", $"```\n{description}\n```", helpMessage);
        }

        private string NormalizeCommandName(string commandName)
        {
            return commandName.ToLowerInvariant().TrimStart('-').Replace("command", string.Empty).Replace(" ", string.Empty);
        }
    }
}
