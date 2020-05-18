using Devscord.DiscordFramework.Framework.Commands.Builders;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public string RenderTextTemplate(BotCommandTemplate template)
        {
            return this.botCommandsTemplateRenderingService.RenderTextTemplate(template);
        }

        public bool IsMatchedWithCommand(DiscordRequest request, BotCommandTemplate template)
        {
            return this.botCommandMatchingService.IsMatchedWithCommand(request, template);
        }

        public BotCommandTemplate GetCommandTemplate(Type commandType)
        {
            return botCommandsTemplateBuilder.GetCommandTemplate(commandType);
        }

        public T ParseRequestToCommand<T>(DiscordRequest request, BotCommandTemplate template) where T : IBotCommand
        {
            return (T)ParseRequestToCommand(typeof(T), request, template);
        }

        public IBotCommand ParseRequestToCommand(Type commandType, DiscordRequest request, BotCommandTemplate template)
        {
            return this.botCommandParsingService.ParseRequestToCommand(commandType, request, template);
        }

        public T ParseCustomTemplate<T>(BotCommandTemplate template, Regex customTemplate, string input) where T : IBotCommand
        {
            return (T)this.botCommandParsingService.ParseCustomTemplate(typeof(T), template, customTemplate, input);
        }

        public IBotCommand ParseCustomTemplate(Type commandType, BotCommandTemplate template, Regex customTemplate, string input)
        {
            return this.botCommandParsingService.ParseCustomTemplate(commandType, template, customTemplate, input);
        }
    }
}
