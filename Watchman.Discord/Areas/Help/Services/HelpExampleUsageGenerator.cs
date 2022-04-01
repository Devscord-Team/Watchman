﻿using System;
using System.Linq;
using System.Text;
using Devscord.DiscordFramework.Commands.PropertyAttributes;
using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Watchman.Discord.ResponsesManagers;
using Watchman.DomainModel.Help;

namespace Watchman.Discord.Areas.Help.Services
{
    public interface IHelpExampleUsageGenerator
    {
        string GetExampleUsage(HelpInformation helpInformation, DiscordServerContext server);
        string GetExampleValue(ArgumentInformation argument, DiscordServerContext server);
    }

    public class HelpExampleUsageGenerator : IHelpExampleUsageGenerator
    {
        private readonly IResponsesService _responsesService;

        public HelpExampleUsageGenerator(IResponsesService responsesService)
        {
            this._responsesService = responsesService;
        }

        public string GetExampleUsage(HelpInformation helpInformation, DiscordServerContext server)
        {
            var exampleBuilder = new StringBuilder();
            exampleBuilder.Append('-');
            exampleBuilder.Append(helpInformation.CommandName.ToLowerInvariant().Replace("command", string.Empty));
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
            var exampleValue = argument.ExpectedTypeName switch
            {
                nameof(Bool) => "",
                nameof(ChannelMention) => this._responsesService.GetResponse(server.Id, x => x.ExampleChannelMention()),
                nameof(List) => this._responsesService.GetResponse(server.Id, x => x.ExampleList()),
                nameof(Number) => "20191027",
                nameof(SingleWord) => this._responsesService.GetResponse(server.Id, x => x.ExampleSingleWord()),
                nameof(Text) => this._responsesService.GetResponse(server.Id, x => x.ExampleText()),
                nameof(Time) => "30m",
                nameof(UserMention) => this._responsesService.GetResponse(server.Id, x => x.ExampleUserMention()),
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