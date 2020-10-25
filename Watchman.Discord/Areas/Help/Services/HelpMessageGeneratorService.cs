using Devscord.DiscordFramework.Framework.Commands.Responses;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;
using Watchman.DomainModel.Help;

namespace Watchman.Discord.Areas.Help.Services
{
    public class HelpMessageGeneratorService
    {
        private readonly ResponsesService _responsesService;

        public HelpMessageGeneratorService(ResponsesService responsesService)
        {
            this._responsesService = responsesService;
        }

        public string GenerateJsonHelp(IEnumerable<HelpInformation> helpInformations)
        {
            var serialized = JsonConvert.SerializeObject(helpInformations, Formatting.Indented);
            serialized = this.RemoveFirstAndLastBracket(serialized);
            return serialized;
        }

        public IEnumerable<KeyValuePair<string, string>> MapToEmbedInput(IEnumerable<HelpInformation> helpInformations)
        {
            foreach (var helpInfo in helpInformations)
            {
                var name = "-" + helpInfo.CommandName.ToLowerInvariant().Replace("command", "");

                var description = helpInfo.Descriptions
                    .FirstOrDefault(x => x.Language == helpInfo.DefaultLanguage)?.Text ?? "Brak domyślnego opisu";

                var descriptionBuilder = new StringBuilder();
                descriptionBuilder.AppendLine(description);

                descriptionBuilder.AppendLine("Przykład użycia: "); // todo: use ReponsesService
                descriptionBuilder.Append(string.IsNullOrWhiteSpace(helpInfo.ExampleUsage)
                    ? this.GetExampleUsage(helpInfo)
                    : helpInfo.ExampleUsage);

                yield return new KeyValuePair<string, string>(name, descriptionBuilder.ToString());
            }
        }

        private string RemoveFirstAndLastBracket(string fullMessage) // '[' & ']'
        {
            return fullMessage[3..^3];
        }

        private string GetExampleUsage(HelpInformation helpInformation) 
            // to review: może wydzielić to do osobnego serwisu? chociaż nie wiem czy jest sens te dwie metody, bo ten serwis nie jest jakiś duży
        {
            var exampleBuilder = new StringBuilder();
            exampleBuilder.Append('-');
            exampleBuilder.Append(helpInformation.CommandName.ToLowerInvariant().Replace("command", ""));
            foreach (var argument in helpInformation.ArgumentInformations)
            {
                exampleBuilder.Append(" -");
                exampleBuilder.Append(this.GetExampleValue(argument));
            }
            return exampleBuilder.ToString();
        }

        private string GetExampleValue(ArgumentInformation argument)
        {
            var exampleValue = argument.ExpectedTypeName switch
            {
                nameof(Bool) => "",
                nameof(ChannelMention) => $"<#{123}>",
                nameof(List) => "jeden dwa trzy",
                nameof(Number) => "20191027",
                nameof(SingleWord) => "pojedynczeSłowo",
                nameof(Text) => "dłuższy teksty",
                nameof(Time) => "30m",
                nameof(UserMention) => $"<@{12345}>",
                _ => "NotImplementedType"
            };
            return string.IsNullOrWhiteSpace(exampleValue)
                ? argument.Name.ToLowerInvariant()
                : $"{argument.Name.ToLowerInvariant()} {exampleValue}";
        }
    }
}
