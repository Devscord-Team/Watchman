using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Discord.Areas.Help.Services;
using Devscord.DiscordFramework.Commons;
using Watchman.DomainModel.Help.Queries;
using Watchman.Cqrs;

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
            _helpMessageGenerator = messageGeneratorService;
            _queryBus = queryBus;
        }

        [DiscordCommand("help")]
        public void PrintHelp(DiscordRequest request, Contexts contexts)
        {
            var messagesService = _messagesServiceFactory.Create(contexts);
            var helpInformations = this._queryBus.Execute(new GetHelpInformationQuery(contexts.Server.Id)).HelpInformations;

            if (request.HasArgument(null, "json"))
            {
                var helpMessage = this._helpMessageGenerator.GenerateJsonHelp(helpInformations);
                messagesService.SendMessage(helpMessage, MessageType.Json);
            }
            else
            {
                messagesService.SendEmbedMessage("Dostępne komendy:", "Poniżej znajdziesz listę dostępnych komend wraz z ich opisami\nJeśli chcesz poznać dokładniejszy opis - zapraszamy do opisu w naszej dokumentacji\nhttps://watchman.readthedocs.io/pl/latest/156-lista-funkcjonalnosci/", _helpMessageGenerator.MapToEmbedInput(helpInformations)); //todo add to responses - now we cannot handle this format
            }
        }
    }
}
