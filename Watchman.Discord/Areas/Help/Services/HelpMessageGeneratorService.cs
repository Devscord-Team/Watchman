using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
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

        public IEnumerable<KeyValuePair<string, string>> MapHelpForAllCommandsToEmbed(IEnumerable<HelpInformation> helpInformations, DiscordServerContext server)
        {
            var areas = helpInformations.GroupBy(x => x.AreaName);
            var noDefaultDescriptionResponse = this._responsesService.GetResponse(server.Id, x => x.NoDefaultDescription());
            foreach (var area in areas)
            {
                var helpBuilder = new StringBuilder();
                foreach (var helpInfo in area)
                {
                    var name = "-" + helpInfo.CommandName.ToLowerInvariant().Replace("command", "");
                    var description = helpInfo.Descriptions
                        .FirstOrDefault(x => x.Language == helpInfo.DefaultLanguage)?.Text ?? noDefaultDescriptionResponse;

                    helpBuilder.AppendLine($"**{name}**");
                    helpBuilder.AppendLine(description);
                    helpBuilder.AppendLine();
                }
                helpBuilder.AppendLine("||-------------||");
                yield return new KeyValuePair<string, string>(area.Key, helpBuilder.ToString());
            }
        }

        public IEnumerable<KeyValuePair<string, string>> MapHelpForOneCommandToEmbed(HelpInformation helpInformation, DiscordServerContext server)
        {
            var helpBuilder = new StringBuilder();
            var typeResponse = this._responsesService.GetResponse(server.Id, x => x.Type());
            var exampleResponse = this._responsesService.GetResponse(server.Id, x => x.Example());
            foreach (var argument in helpInformation.ArgumentInformations)
            {
                helpBuilder.AppendLine($"**{argument.Name}**");
                helpBuilder.AppendLine($"```{typeResponse.ToLowerInvariant()}: {argument.ExpectedTypeName}");
                var exampleValue = argument.ExampleValue ?? this._helpExampleUsageGenerator.GetExampleValue(argument);
                helpBuilder.AppendLine(!string.IsNullOrWhiteSpace(exampleValue) 
                    ? $"{exampleResponse.ToLowerInvariant()}: {exampleValue}```" 
                    : "```");
            }
            var parametersResponse = this._responsesService.GetResponse(server.Id, x => x.Parameters());
            yield return new KeyValuePair<string, string>($"__{parametersResponse}__", helpBuilder.ToString());
            var exampleCommandUsage = helpInformation.ExampleUsage ?? this._helpExampleUsageGenerator.GetExampleUsage(helpInformation);
            yield return new KeyValuePair<string, string>($"__{exampleResponse}__", exampleCommandUsage);
        }

        private string RemoveFirstAndLastBracket(string fullMessage) // '[' & ']'
        {
            return fullMessage[3..^3];
        }
    }
}
