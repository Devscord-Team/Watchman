using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devscord.DiscordFramework.Commons.Extensions;
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

        private string RemoveFirstAndLastBracket(string fullMessage) // '[' & ']'
        {
            return fullMessage[3..^3];
        }
    }
}
