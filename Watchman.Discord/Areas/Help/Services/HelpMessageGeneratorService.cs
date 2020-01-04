using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Newtonsoft.Json;
using Watchman.Cqrs;
using Watchman.DomainModel.Help.Queries;

namespace Watchman.Discord.Areas.Help.Services
{
    public class HelpMessageGeneratorService : IService
    {
        const int MAX_MESSAGE_LENGTH = 1950; // for safety reason I made it smaller than 2000

        private readonly IQueryBus _queryBus;

        public HelpMessageGeneratorService(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        public string GenerateHelp(Contexts contexts)
        {
            var result = this._queryBus.Execute(new GetHelpInformationQuery(contexts.Server.Id));

            var messageBuilder = new StringBuilder();
            foreach (var helpInfo in result.HelpInformations)
            {
                var line = new StringBuilder("-" + helpInfo.Names.Aggregate((x, y) => $"{x} / -{y}"));
                line.Append(" => ");
                line.Append(helpInfo.Descriptions.First(x => x.Name == helpInfo.DefaultDescriptionName).Details);
                messageBuilder.AppendLine(line.ToString());
            }

            return messageBuilder.ToString();
        }

        public IEnumerable<string> GenerateJsonHelp(Contexts contexts)
        {
            var result = this._queryBus.Execute(new GetHelpInformationQuery(contexts.Server.Id));
            var serialized = JsonConvert.SerializeObject(result.HelpInformations, Formatting.Indented);
            var smallerMessages = SplitJsonToSmallerMessages(serialized);

            foreach (var sm in smallerMessages)
            {
                var messageBuilder = new StringBuilder();
                messageBuilder.AppendLine("```json");
                messageBuilder.AppendLine(sm);
                messageBuilder.AppendLine("```");
                yield return messageBuilder.ToString();
            }
        }

        private IEnumerable<string> SplitJsonToSmallerMessages(string message)
        {
            while (message.Length > MAX_MESSAGE_LENGTH)
            {
                var cutIndex = message.Substring(0, MAX_MESSAGE_LENGTH).LastIndexOf("},", StringComparison.Ordinal) + 2;
                yield return message.Substring(0, cutIndex);
                message = message.Remove(0, cutIndex);
            }
            yield return message;
        }

    }
}
