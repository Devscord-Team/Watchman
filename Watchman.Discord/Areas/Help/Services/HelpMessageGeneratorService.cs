using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Watchman.DomainModel.Help;

namespace Watchman.Discord.Areas.Help.Services
{
    public class HelpMessageGeneratorService
    {
        public string GenerateJsonHelp(IEnumerable<HelpInformation> helpInformations)
        {
            var serialized = JsonConvert.SerializeObject(helpInformations, Formatting.Indented);
            serialized = RemoveFirstAndLastBracket(serialized);
            return serialized;
        }

        public IEnumerable<KeyValuePair<string, string>> MapToEmbedInput(IEnumerable<HelpInformation> helpInformations)
        {
            foreach(var helpInfo in helpInformations)
            {
                var name = "-" + helpInfo.Names.Aggregate((a, b) => $"{a} / {b}");

                var descriptions = helpInfo.Descriptions
                    .Where(x => x.Details.Trim().ToLowerInvariant() != "empty")
                    .Select(x => $"{x.Name} => {x.Details}")
                    .ToList();

                var arguments = helpInfo.ArgumentInfos
                    .Where(x => x.Description.Trim().ToLowerInvariant() != "empty")
                    .Select(x => $"{x.Name} => {x.Description}")
                    .ToList();

                var content = string.Empty;
                if(descriptions.Any())
                {
                    var commandDescription = descriptions.Aggregate((a, b) => a + "\n" + b);
                    content = commandDescription + "\n\n";
                }
                if(arguments.Any())
                {
                    content += arguments.Aggregate((a, b) => a + "\n" + b);
                }

                if (string.IsNullOrWhiteSpace(content))
                {
                    content = "Jeszcze nie posiada opisu";
                }
                yield return new KeyValuePair<string, string>(name, content);
            }
        }

        private string RemoveFirstAndLastBracket(string fullMessage) // '[' & ']'
        {
            return fullMessage[3..^3];
        }
    }
}
