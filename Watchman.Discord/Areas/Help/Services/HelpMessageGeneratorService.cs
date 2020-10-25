using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Watchman.DomainModel.Help;

namespace Watchman.Discord.Areas.Help.Services
{
    public class HelpMessageGeneratorService
    {
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
                var name = "-" + helpInfo.CommandName;

                var description = helpInfo.Descriptions
                    .FirstOrDefault(x => x.Language == helpInfo.DefaultLanguage).Text ?? "Empty";

                //var arguments = helpInfo.ArgumentInformations
                //    .Where(x => x.Description.Trim().ToLowerInvariant() != "empty")
                //    .Select(x => $"{x.Name} => {x.Description}")
                //    .ToList();

                var content = $"{name} => {description}";
                //if (descriptions.Any())
                //{
                //    var commandDescription = descriptions.Aggregate((a, b) => a + "\n" + b);
                //    content = commandDescription + "\n\n";
                //}
                //if (arguments.Any())
                //{
                //    content += arguments.Aggregate((a, b) => a + "\n" + b);
                //}

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
