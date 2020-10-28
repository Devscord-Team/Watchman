using System.Linq;
using System.Text;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;
using Watchman.DomainModel.Help;

namespace Watchman.Discord.Areas.Help.Services
{
    public class HelpExampleUsageGenerator
    {
        public string GetExampleUsage(HelpInformation helpInformation)
        {
            var exampleBuilder = new StringBuilder();
            exampleBuilder.Append('-');
            exampleBuilder.Append(helpInformation.CommandName.ToLowerInvariant().Replace("command", ""));
            var arguments = helpInformation.ArgumentInformations.Where(x => !x.IsOptional).ToList();
            if (helpInformation.ArgumentInformations.Any(x => x.IsOptional))
            {
                arguments.Add(helpInformation.ArgumentInformations.First(x => x.IsOptional));
            }
            foreach (var argument in arguments)
            {
                exampleBuilder.Append(" -");
                exampleBuilder.Append(this.GetExampleArgument(argument));
            }
            return exampleBuilder.ToString();
        }

        public string GetExampleValue(ArgumentInformation argument)
        {
            var exampleValue = argument.ExpectedTypeName switch
            {
                nameof(Bool) => "",
                nameof(ChannelMention) => $"<#{123}>",
                nameof(List) => "jeden dwa trzy",
                nameof(Number) => "20191027",
                nameof(SingleWord) => "pojedynczeSłowo",
                nameof(Text) => "\"kilka słów\"",
                nameof(Time) => "30m",
                nameof(UserMention) => $"<@{12345}>",
                _ => "NotImplementedType"
            };
            return exampleValue;
        }

        private string GetExampleArgument(ArgumentInformation argumentInformation)
        {
            var exampleValue = this.GetExampleValue(argumentInformation);
            return string.IsNullOrWhiteSpace(exampleValue)
                ? argumentInformation.Name.ToLowerInvariant()
                : $"{argumentInformation.Name.ToLowerInvariant()} {exampleValue}";
        }
    }
}