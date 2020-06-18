using Devscord.DiscordFramework.Framework.Commands.Builders;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using System;
using System.Text.RegularExpressions;

namespace Devscord.DiscordFramework.Framework.Commands.Services
{
    public class BotCommandsService
    {
        private readonly BotCommandsTemplateBuilder botCommandsTemplateBuilder;
        private readonly BotCommandsParsingService botCommandParsingService;
        private readonly BotCommandsMatchingService botCommandMatchingService;
        private readonly BotCommandsTemplateRenderingService botCommandsTemplateRenderingService;

        public BotCommandsService(BotCommandsTemplateBuilder botCommandsTemplateBuilder, BotCommandsParsingService botCommandParsingService,
            BotCommandsMatchingService botCommandMatchingService, BotCommandsTemplateRenderingService botCommandsTemplateRenderingService)
        {
            this.botCommandsTemplateBuilder = botCommandsTemplateBuilder;
            this.botCommandParsingService = botCommandParsingService;
            this.botCommandMatchingService = botCommandMatchingService;
            this.botCommandsTemplateRenderingService = botCommandsTemplateRenderingService;
        }

        public string RenderTextTemplate(BotCommandTemplate template) => this.botCommandsTemplateRenderingService.RenderTextTemplate(template);

        public bool IsMatchedWithCommand(DiscordRequest request, BotCommandTemplate template) => this.botCommandMatchingService.IsMatchedWithCommand(request, template);

        public BotCommandTemplate GetCommandTemplate(Type commandType) => this.botCommandsTemplateBuilder.GetCommandTemplate(commandType);

        public T ParseRequestToCommand<T>(DiscordRequest request, BotCommandTemplate template) where T : IBotCommand => (T)this.ParseRequestToCommand(typeof(T), request, template);

        public IBotCommand ParseRequestToCommand(Type commandType, DiscordRequest request, BotCommandTemplate template) => this.botCommandParsingService.ParseRequestToCommand(commandType, request, template);

        public IBotCommand ParseCustomTemplate(Type commandType, BotCommandTemplate template, Regex customTemplate, string input) => this.botCommandParsingService.ParseCustomTemplate(commandType, template, customTemplate, input);
    }
}
