using System.Linq;
using System.Text;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Newtonsoft.Json;
using Watchman.Cqrs;
using Watchman.DomainModel.Help.Queries;

namespace Watchman.Discord.Areas.Help.Services
{
    public class HelpMessageGeneratorService
    {
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

        public string GenerateJsonHelp(Contexts contexts)
        {
            var result = this._queryBus.Execute(new GetHelpInformationQuery(contexts.Server.Id));

            var serialized = JsonConvert.SerializeObject(result.HelpInformations, Formatting.Indented);
            serialized = RemoveFirstAndLastBracket(serialized);
            return serialized;
        }

        private string RemoveFirstAndLastBracket(string fullMessage) // '[' & ']'
        {
            return fullMessage[3..^3];
        }
    }
}
