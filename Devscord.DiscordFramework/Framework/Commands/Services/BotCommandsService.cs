using Devscord.DiscordFramework.Framework.Commands.Builders;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        public bool IsDefaultCommand(BotCommandTemplate template, IEnumerable<DiscordRequestArgument> arguments, bool isCommandMatchedWithCustom)
        {
            return this.botCommandMatchingService.IsDefaultCommand(template, arguments, isCommandMatchedWithCustom);
        }

        public bool AreDefaultCommandArgumentsCorrect(BotCommandTemplate template, IEnumerable<DiscordRequestArgument> arguments)
        {
            return this.botCommandMatchingService.AreDefaultCommandArgumentsCorrect(template, arguments);
        }

        public bool AreCustomCommandArgumentsCorrect(BotCommandTemplate template, Regex customTemplate, string input)
        {
            return this.botCommandMatchingService.AreCustomCommandArgumentsCorrect(template, customTemplate, input);
        }

        public BotCommandTemplate GetCommandTemplate(Type commandType)
        {
            return this.botCommandsTemplateBuilder.GetCommandTemplate(commandType);
        }

        public T ParseRequestToCommand<T>(DiscordRequest request, BotCommandTemplate template) where T : IBotCommand
        {
            return (T) this.ParseRequestToCommand(typeof(T), request, template);
        }

        public IBotCommand ParseRequestToCommand(Type commandType, DiscordRequest request, BotCommandTemplate template)
        {
            return this.botCommandParsingService.ParseRequestToCommand(commandType, request, template);
        }

        public IBotCommand ParseCustomTemplate(Type commandType, BotCommandTemplate template, Regex customTemplate, string input)
        {
            return this.botCommandParsingService.ParseCustomTemplate(commandType, template, customTemplate, input);
        }
    }
}
