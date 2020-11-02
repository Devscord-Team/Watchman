using System;
using System.Linq;
using System.Text;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Watchman.DomainModel.Help;

namespace Watchman.Discord.Areas.Help.Services
{
    public class HelpExampleUsageGenerator
    {
        private readonly ResponsesService _responsesService;

        public HelpExampleUsageGenerator(ResponsesService responsesService)
        {
            this._responsesService = responsesService;
        }

        public string GetExampleUsage(HelpInformation helpInformation, DiscordServerContext server)
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
                exampleBuilder.Append(this.GetExampleArgument(argument, server));
            }
            return exampleBuilder.ToString();
        }

        public string GetExampleValue(ArgumentInformation argument, DiscordServerContext server)
        {
            string getResponse(Func<ResponsesService, string> x) => this._responsesService.GetResponse(server.Id, x);
            var exampleValue = argument.ExpectedTypeName switch
            {
                nameof(Bool) => "",
                nameof(ChannelMention) => getResponse(x => x.ExampleChannelMention()),
                nameof(List) => getResponse(x => x.ExampleList()),
                nameof(Number) => "20191027",
                nameof(SingleWord) => getResponse(x => x.ExampleSingleWord()),
                nameof(Text) => getResponse(x => x.ExampleText()),
                nameof(Time) => "30m",
                nameof(UserMention) => getResponse(x => x.ExampleUserMention()),
                _ => "NotImplementedType"
            };
            return exampleValue;
        }

        private string GetExampleArgument(ArgumentInformation argumentInformation, DiscordServerContext server)
        {
            var exampleValue = this.GetExampleValue(argumentInformation, server);
            return string.IsNullOrWhiteSpace(exampleValue)
                ? argumentInformation.Name.ToLowerInvariant()
                : $"{argumentInformation.Name.ToLowerInvariant()} {exampleValue}";
        }
    }
}