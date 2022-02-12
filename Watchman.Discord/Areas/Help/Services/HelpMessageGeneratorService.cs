using Devscord.DiscordFramework.Commands.Responses;
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
            var areas = helpInformations.GroupBy(x => x.AreaName).ToList();
            foreach (var area in areas.SkipLast(1))
            {
                var helpBuilder = this.GetBasicDescriptionForArea(area, server.Id);
                helpBuilder.AppendLine("||-------------||");
                yield return new KeyValuePair<string, string>(area.Key, helpBuilder.ToString());
            }
            var builder = this.GetBasicDescriptionForArea(areas.Last(), server.Id);
            yield return new KeyValuePair<string, string>(areas.Last().Key, builder.ToString());
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
                var exampleValue = argument.ExampleValue ?? this._helpExampleUsageGenerator.GetExampleValue(argument, server);
                helpBuilder.AppendLine(!string.IsNullOrWhiteSpace(exampleValue) 
                    ? $"{exampleResponse.ToLowerInvariant()}: {exampleValue}```" 
                    : "```");
            }
            var parametersResponse = this._responsesService.GetResponse(server.Id, x => x.Parameters());
            yield return new KeyValuePair<string, string>($"__{parametersResponse}__", helpBuilder.ToString());
            var exampleCommandUsage = helpInformation.ExampleUsage ?? this._helpExampleUsageGenerator.GetExampleUsage(helpInformation, server);
            yield return new KeyValuePair<string, string>($"__{exampleResponse}__", exampleCommandUsage);
        }

        private string RemoveFirstAndLastBracket(string fullMessage) // '[' & ']'
        {
            return fullMessage[3..^3];
        }

        private StringBuilder GetBasicDescriptionForArea(IEnumerable<HelpInformation> area, ulong serverId)
        {
            var helpBuilder = new StringBuilder();
            foreach (var helpInfo in area)
            {
                helpBuilder = this.GetBasicDescriptionForCommand(helpBuilder, helpInfo, serverId);
            }
            return helpBuilder;
        }

        private StringBuilder GetBasicDescriptionForCommand(StringBuilder helpBuilder, HelpInformation helpInformation, ulong serverId)
        {
            var noDefaultDescriptionResponse = this._responsesService.GetResponse(serverId, x => x.NoDefaultDescription());
            var name = "-" + helpInformation.CommandName.ToLowerInvariant().Replace("command", string.Empty);
            var description = helpInformation.Descriptions
                .FirstOrDefault(x => x.Language == helpInformation.DefaultLanguage)?.Text ?? noDefaultDescriptionResponse;

            helpBuilder.AppendLine($"**{name}**");
            helpBuilder.AppendLine(description);
            helpBuilder.AppendLine();
            return helpBuilder;
        }
    }
}
