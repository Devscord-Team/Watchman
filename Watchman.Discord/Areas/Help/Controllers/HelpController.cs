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
using Watchman.Integrations.MongoDB;
using Devscord.DiscordFramework.Framework.Commands.Responses;

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
            if (request.Arguments.Any() && request.Arguments.First().Values.Any(x => x == "json"))
            { 
                PrintJsonHelp(request, contexts);
                return;
            }
            
            var result = this._queryBus.Execute(new GetHelpInformationQuery(contexts.Server.Id));

            var lines = new List<string>();
            foreach (var helpInfo in result.HelpInformations)
            {
                var line = new StringBuilder("-" + helpInfo.Names.Aggregate((x, y) => x + " / -" + y));

                line.Append(" => ");
                line.Append(helpInfo.Descriptions.First(x => x.Name == helpInfo.DefaultDescriptionName).Details);
                lines.Add(line.ToString());
            }

            var messageBuilder = new StringBuilder().PrintManyLines(lines.ToArray());
            var messagesService = _messagesServiceFactory.Create(contexts);
            messagesService.SendResponse(x => x.PrintHelp(messageBuilder.ToString()), contexts);
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
            messagesService.SendResponse(x => x.PrintHelp(messageBuilder.ToString()), contexts);
        }
    }
}
