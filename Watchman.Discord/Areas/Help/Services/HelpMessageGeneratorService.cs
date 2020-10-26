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

        public IEnumerable<KeyValuePair<string, string>> MapToEmbedInput(IEnumerable<HelpInformation> helpInformations)
        {
            var areas = helpInformations.GroupBy(x => x.AreaName);
            foreach (var area in areas)
            {
                foreach (var helpInfo in area)
                {
                    var name = "-" + helpInfo.CommandName.ToLowerInvariant().Replace("command", "");

                    var description = helpInfo.Descriptions
                        .FirstOrDefault(x => x.Language == helpInfo.DefaultLanguage)?.Text ?? "Brak domyślnego opisu";

                    var descriptionBuilder = new StringBuilder();
                    descriptionBuilder.AppendLine(description);
                    yield return new KeyValuePair<string, string>(name, descriptionBuilder.ToString());
                }
            }
        }

        private string RemoveFirstAndLastBracket(string fullMessage) // '[' & ']'
        {
            return fullMessage[3..^3];
        }
    }
}
