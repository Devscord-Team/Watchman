using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Newtonsoft.Json;
using PCRE;
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
            serialized = RemoveFirstAndLastBracket(serialized);
            serialized = TrimUselessWhitespaceFromBeginning(serialized);

            foreach (var sm in SplitJsonToSmallerMessages(serialized))
            {
                var messageBuilder = new StringBuilder();
                messageBuilder.AppendLine("```json");
                messageBuilder.AppendLine(sm);
                messageBuilder.AppendLine("```");
                yield return messageBuilder.ToString();
            }
        }

        private string RemoveFirstAndLastBracket(string fullMessage)
        {
            return fullMessage[3..^3];
        }

        private string TrimUselessWhitespaceFromBeginning(string fullMessage)
        {
            var lines = fullMessage.Split('\n');
            var howManyWhitespaces = lines.First().IndexOf('{');

            return lines.Aggregate((sum, next) => sum + next.Remove(0, howManyWhitespaces));
        }

        private IEnumerable<string> SplitJsonToSmallerMessages(string fullMessage)
        {
            var matched = new PcreRegex(@"{(?>[^{}]|(?R))*}").Matches(fullMessage).Select(x => x.Value);

            var oneMessage = new StringBuilder();
            foreach (var jsonElement in matched)
            {
                if (jsonElement.Length + oneMessage.Length > MAX_MESSAGE_LENGTH)
                {
                    yield return oneMessage.ToString();
                    oneMessage.Clear();
                }
                oneMessage.AppendLine(jsonElement + ',');
            }

            yield return oneMessage.ToString()[..^3]; // skip last ",\n"
        }

    }
}
