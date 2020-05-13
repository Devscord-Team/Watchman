using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Newtonsoft.Json;
using Watchman.Cqrs;
using Watchman.DomainModel.Help;
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

            var lines = new Dictionary<string, string>();
            foreach (var helpInfo in result.HelpInformations)
            {
                var names = helpInfo.Names.Aggregate((x, y) => $"{x} / -{y}");
                var description = helpInfo.Descriptions.First().Details;

                lines.Add(names, description);
            }

            var messageBuilder = new StringBuilder();
            messageBuilder.PrintManyLines(lines, false);
            return messageBuilder.ToString();
        }

        public string GenerateJsonHelp(Contexts contexts)
        {
            var result = this._queryBus.Execute(new GetHelpInformationQuery(contexts.Server.Id));

            var serialized = JsonConvert.SerializeObject(result.HelpInformations, Formatting.Indented);
            serialized = RemoveFirstAndLastBracket(serialized);
            return serialized;
        }

        public IEnumerable<KeyValuePair<string, string>> MapToEmbedInput(IEnumerable<HelpInformation> helpInformations)
        {
            return helpInformations.Select(x =>
            {
                var name = "-" + x.Names.First();

                var descriptions = x.Descriptions.Where(x => x.Details.Trim().ToLowerInvariant() != "empty").Select(x => $"{x.Name} => {x.Details}");
                var arguments = x.ArgumentInfos.Where(x => x.Description.Trim().ToLowerInvariant() != "empty")?.Select(x => $"{x.Name} => {x.Description}");

                var content = descriptions.Any() ? descriptions.Aggregate((a, b) => a + "\n" + b) : string.Empty;
                if(!string.IsNullOrWhiteSpace(content))
                {
                    content = content + "\n\n";
                }
                if(arguments.Any())
                {
                    content = content + arguments.Aggregate((a, b) => a + "\n" + b);
                }

                if (string.IsNullOrWhiteSpace(content))
                {
                    content = "Jeszcze nie posiada opisu";
                }
                return new KeyValuePair<string, string>(name, content);
            });
        }

        private string RemoveFirstAndLastBracket(string fullMessage) // '[' & ']'
        {
            return fullMessage[3..^3];
        }
    }
}
