using System.Collections.Generic;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Watchman.Cqrs;
using Watchman.DomainModel.Help.Queries;
using Watchman.Integrations.MongoDB;

namespace Watchman.Discord.Areas.Help.Controllers
{
    public class HelpController : IController
    {
        private readonly IQueryBus _queryBus;
        private readonly MessagesServiceFactory _messagesServiceFactory;

        public HelpController(IQueryBus queryBus, MessagesServiceFactory messagesServiceFactory)
        {
            this._queryBus = queryBus;
            this._messagesServiceFactory = messagesServiceFactory;
        }

        [DiscordCommand("help")]
        public void PrintHelp(DiscordRequest request, Contexts contexts)
        {
            if (request.Arguments.First().Values.Any(x => x == "json"))
            { 
                PrintJsonHelp(request, contexts);
                return;
            }
            
            var result = this._queryBus.Execute(new GetHelpInformationQuery(contexts.Server.Id));

            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("```");

            foreach (var helpInfo in result.HelpInformations)
            {
                helpInfo.Names.ToList().ForEach(x => messageBuilder.Append(x).Append(" / "));
                messageBuilder.Remove(messageBuilder.Length - 3, 3);
                
                messageBuilder.Append(" => ");

                messageBuilder.AppendLine(helpInfo.Descriptions.First(x => x.Name == helpInfo.DefaultDescriptionName).Details);
            }
            
            messageBuilder.AppendLine("```");

            var messagesService = _messagesServiceFactory.Create(contexts);
            messagesService.SendMessage(messageBuilder.ToString());
        }

        public void PrintJsonHelp(DiscordRequest request, Contexts contexts)
        {
            var result = this._queryBus.Execute(new GetHelpInformationQuery(contexts.Server.Id));

            var serialized = JsonConvert.SerializeObject(result.HelpInformations, Formatting.Indented);
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("```json");
            messageBuilder.AppendLine(serialized);
            messageBuilder.AppendLine("```");

            var messagesService = _messagesServiceFactory.Create(contexts);
            var message = messageBuilder.ToString();
            messagesService.SendMessage(message);
        }
    }
}
