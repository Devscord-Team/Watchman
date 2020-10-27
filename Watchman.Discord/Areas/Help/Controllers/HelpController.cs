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

namespace Watchman.Discord.Areas.Help.Controllers
{
    public class HelpController : IController
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly HelpMessageGeneratorService _helpMessageGenerator;
        private readonly IQueryBus _queryBus;

        public HelpController(MessagesServiceFactory messagesServiceFactory, HelpMessageGeneratorService messageGeneratorService, IQueryBus queryBus)
        {
            this._messagesServiceFactory = messagesServiceFactory;
            this._helpMessageGenerator = messageGeneratorService;
            this._queryBus = queryBus;
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
            return messagesService.SendEmbedMessage("Dostępne komendy:",
                "Poniżej znajdziesz listę dostępnych komend wraz z ich opisami\nJeśli chcesz poznać dokładniejszy opis - zapraszamy do opisu w naszej dokumentacji\nhttps://watchman.readthedocs.io/pl/latest/156-lista-funkcjonalnosci/",
                this._helpMessageGenerator.MapHelpToEmbed(helpInformations)); //todo add to responses - now we cannot handle this format
        }

        private Task PrintHelpForOneCommand(HelpCommand command, Contexts contexts, IEnumerable<HelpInformation> helpInformations)
        {
            var helpInformation = helpInformations.FirstOrDefault(x => x.CommandName.ToLowerInvariant().Replace("command", "") == command.Command.TrimStart('-'));
            if (helpInformation == null)
            {
                return Task.CompletedTask;
            }
            var messagesService = this._messagesServiceFactory.Create(contexts);
            var helpMessage = this._helpMessageGenerator.MapCommandHelpToEmbed(helpInformation);
            return messagesService.SendEmbedMessage($"Jak używać komendy {command.Command}", "", helpMessage);
        }
    }
}
