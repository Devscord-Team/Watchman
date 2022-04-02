using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Discord.ResponsesManagers;
using Watchman.DomainModel.Help;

namespace Watchman.Discord.Areas.Help.Services
{
    public interface IHelpMessageGeneratorService
    {
        string GenerateJsonHelp(IEnumerable<HelpInformation> helpInformations);
        IEnumerable<KeyValuePair<string, string>> MapHelpForAllCommandsToEmbed(IEnumerable<HelpInformation> helpInformations, DiscordServerContext server);
        IEnumerable<KeyValuePair<string, string>> MapHelpForOneCommandToEmbed(HelpInformation helpInformation, DiscordServerContext server);
    }

    public class HelpMessageGeneratorService : IHelpMessageGeneratorService
    {
        private readonly IResponsesService _responsesService;
        private readonly IHelpExampleUsageGenerator _helpExampleUsageGenerator;

        public HelpMessageGeneratorService(IResponsesService responsesService, IHelpExampleUsageGenerator helpExampleUsageGenerator)
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
            var areas = helpInformations.GroupBy(x => x.AreaName).OrderBy(x => x.Key).ToList();
            foreach (var area in areas.SkipLast(1))
            {
                var helpBuilder = this.GetBasicDescriptionForArea(area, server.Id);
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
                helpBuilder.AppendLine($"{argument.Name}");
                helpBuilder.AppendLine($"```{typeResponse.ToLowerInvariant()}: {argument.ExpectedTypeName}");
                var exampleValue = argument.ExampleValue ?? this._helpExampleUsageGenerator.GetExampleValue(argument, server);
                helpBuilder.AppendLine(!string.IsNullOrWhiteSpace(exampleValue)
                    ? $"{exampleResponse.ToLowerInvariant()}: {exampleValue}```"
                    : "```");
            }
            var parametersResponse = this._responsesService.GetResponse(server.Id, x => x.Parameters());
            var helpBuilderResult = helpBuilder.ToString();
            if (!string.IsNullOrWhiteSpace(helpBuilderResult))
            {
                yield return new KeyValuePair<string, string>($"**{parametersResponse}**", helpBuilderResult);
            }
            var exampleCommandUsage = helpInformation.ExampleUsage ?? this._helpExampleUsageGenerator.GetExampleUsage(helpInformation, server);
            yield return new KeyValuePair<string, string>($"**{exampleResponse}**", exampleCommandUsage);
        }

        private string RemoveFirstAndLastBracket(string fullMessage) // '[' & ']'
        {
            return fullMessage[3..^3];
        }

        private StringBuilder GetBasicDescriptionForArea(IEnumerable<HelpInformation> area, ulong serverId)
        {
            var helpBuilder = new StringBuilder();
            helpBuilder.AppendLine("```");
            foreach (var helpInfo in area)
            {
                helpBuilder = this.GetBasicDescriptionForCommand(helpBuilder, helpInfo, serverId);
            }
            helpBuilder.Append("```");
            return helpBuilder;
        }

        private StringBuilder GetBasicDescriptionForCommand(StringBuilder helpBuilder, HelpInformation helpInformation, ulong serverId)
        {
            var name = "-" + helpInformation.CommandName.ToLowerInvariant().Replace("command", string.Empty);
            helpBuilder.AppendLine(name);
            return helpBuilder;
        }
    }
}
