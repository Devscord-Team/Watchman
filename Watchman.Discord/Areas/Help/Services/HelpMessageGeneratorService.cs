using Devscord.DiscordFramework.Framework.Commands.Responses;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.DomainModel.Help;

namespace Watchman.Discord.Areas.Help.Services
{
    public class HelpMessageGeneratorService
    {
        private readonly ResponsesService _responsesService;
        private readonly HelpExampleUsageGenerator _helpExampleUsageGenerator;

        public HelpMessageGeneratorService(ResponsesService responsesService, HelpExampleUsageGenerator helpExampleUsageGenerator)
        {
            this._responsesService = responsesService;
            this._helpExampleUsageGenerator = helpExampleUsageGenerator;
        }

        public string GenerateJsonHelp(IEnumerable<HelpInformation> helpInformations)
        {
            var serialized = JsonConvert.SerializeObject(helpInformations, Formatting.Indented);
            serialized = this.RemoveFirstAndLastBracket(serialized);
            return serialized;
        }

        public IEnumerable<KeyValuePair<string, string>> MapHelpToEmbed(IEnumerable<HelpInformation> helpInformations)
        {
            var areas = helpInformations.GroupBy(x => x.AreaName);
            foreach (var area in areas)
            {
                var helpBuilder = new StringBuilder();
                foreach (var helpInfo in area)
                {
                    var name = "-" + helpInfo.CommandName.ToLowerInvariant().Replace("command", "");
                    var description = helpInfo.Descriptions
                        .FirstOrDefault(x => x.Language == helpInfo.DefaultLanguage)?.Text ?? "Brak domyślnego opisu";

                    helpBuilder.AppendLine($"**{name}**");
                    helpBuilder.AppendLine(description);
                    helpBuilder.AppendLine();
                }
                helpBuilder.AppendLine("----------");
                yield return new KeyValuePair<string, string>(area.Key, helpBuilder.ToString());
            }
        }

        public IEnumerable<KeyValuePair<string, string>> MapCommandHelpToEmbed(HelpInformation helpInformation)
        {
            var helpBuilder = new StringBuilder();
            var description = helpInformation.Descriptions.
                FirstOrDefault(x => x.Language == helpInformation.DefaultLanguage)?.Text ?? "Brak domyślnego opisu";
            yield return new KeyValuePair<string, string>("Opis", description);

            foreach (var argument in helpInformation.ArgumentInformations)
            {
                helpBuilder.AppendLine();
                helpBuilder.AppendLine($"**{argument.Name}**");
                helpBuilder.AppendLine($"typ: {argument.ExpectedTypeName}");
                var exampleValue = argument.ExampleValue ?? this._helpExampleUsageGenerator.GetExampleValue(argument);
                helpBuilder.AppendLine($"przykład: {exampleValue}");
            }
            yield return new KeyValuePair<string, string>("Parametry", helpBuilder.ToString());
            var exampleCommandUsage = helpInformation.ExampleUsage ?? this._helpExampleUsageGenerator.GetExampleUsage(helpInformation);
            yield return new KeyValuePair<string, string>("Przykład", exampleCommandUsage);
        }

        private string RemoveFirstAndLastBracket(string fullMessage) // '[' & ']'
        {
            return fullMessage[3..^3];
        }
    }
}
