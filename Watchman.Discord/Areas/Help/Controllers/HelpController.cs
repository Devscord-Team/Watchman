using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Help.BotCommands;
<<<<<<< HEAD
using Watchman.Discord.Areas.Help.Services;
using Watchman.DomainModel.Help.Queries;
=======
using System.Threading.Tasks;
using Watchman.Integrations.Google;
using System.Linq;
using System.Collections.Generic;
>>>>>>> master

namespace Watchman.Discord.Areas.Help.Controllers
{
    public class HelpController : IController
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly HelpMessageGeneratorService _helpMessageGenerator;
        private readonly IQueryBus _queryBus;
        private readonly GoogleSearchService _googleSearchService;

        public HelpController(MessagesServiceFactory messagesServiceFactory, HelpMessageGeneratorService messageGeneratorService, IQueryBus queryBus, GoogleSearchService googleSearchService)
        {
            this._messagesServiceFactory = messagesServiceFactory;
            this._helpMessageGenerator = messageGeneratorService;
            this._queryBus = queryBus;
<<<<<<< HEAD
=======
            this._googleSearchService = googleSearchService;
>>>>>>> master
        }

        public async Task PrintHelp(HelpCommand command, Contexts contexts)
        {
            var messagesService = this._messagesServiceFactory.Create(contexts);
            var helpInformations = this._queryBus.Execute(new GetHelpInformationQuery(contexts.Server.Id)).HelpInformations;

            if (command.Json)
            {
                var helpMessage = this._helpMessageGenerator.GenerateJsonHelp(helpInformations);
                await messagesService.SendMessage(helpMessage, MessageType.Json);
            }
            else
            {
<<<<<<< HEAD
                messagesService.SendEmbedMessage("Dostępne komendy:", "Poniżej znajdziesz listę dostępnych komend wraz z ich opisami\nJeśli chcesz poznać dokładniejszy opis - zapraszamy do opisu w naszej dokumentacji\nhttps://watchman.readthedocs.io/pl/latest/156-lista-funkcjonalnosci/", this._helpMessageGenerator.MapToEmbedInput(helpInformations)); //todo add to responses - now we cannot handle this format
=======
                await messagesService.SendEmbedMessage("Dostępne komendy:", "Poniżej znajdziesz listę dostępnych komend wraz z ich opisami\nJeśli chcesz poznać dokładniejszy opis - zapraszamy do opisu w naszej dokumentacji\nhttps://watchman.readthedocs.io/pl/latest/156-lista-funkcjonalnosci/", this._helpMessageGenerator.MapToEmbedInput(helpInformations)); //todo add to responses - now we cannot handle this format
>>>>>>> master
            }
        }

        public async Task GoogleSearch(GoogleCommand command, Contexts contexts)
        {
            var messagesService = this._messagesServiceFactory.Create(contexts);
            var results = this._googleSearchService.Search(command.Search);
            await messagesService.SendEmbedMessage($"Wyniki google dla zapytania {command.Search}", "", results.Select(x => new KeyValuePair<string,string>( x.Title, x.Url + "\n" + x.Description)));
        }
    }
}
