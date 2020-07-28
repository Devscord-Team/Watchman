using Devscord.DiscordFramework.Framework.Commands.Builders;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using System;
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

        public bool IsMatchedWithCommand(DiscordRequest request, BotCommandTemplate template)
        {
            return this.botCommandMatchingService.IsMatchedWithCommand(request, template);
        }

        public BotCommandTemplate GetCommandTemplate(Type commandType)
        {
            return this.botCommandsTemplateBuilder.GetCommandTemplate(commandType);
        }

        public async Task<T> ParseRequestToCommand<T>(DiscordRequest request, BotCommandTemplate template) where T : IBotCommand
        {
            return (T) await this.ParseRequestToCommand(typeof(T), request, template);
        }

        public async Task<IBotCommand> ParseRequestToCommand(Type commandType, DiscordRequest request, BotCommandTemplate template)
        {
            return await this.botCommandParsingService.ParseRequestToCommand(commandType, request, template);
        }

        public async Task<IBotCommand> ParseCustomTemplate(Type commandType, BotCommandTemplate template, Regex customTemplate, string input)
        {
            return await this.botCommandParsingService.ParseCustomTemplate(commandType, template, customTemplate, input);
        }
    }
}
