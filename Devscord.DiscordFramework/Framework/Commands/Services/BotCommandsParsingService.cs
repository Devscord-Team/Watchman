using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Serilog;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Devscord.DiscordFramework.Framework.Commands.Properties;
using Devscord.DiscordFramework.Commons.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Framework.Commands.Services
{
    public class BotCommandsParsingService
    {
        private readonly BotCommandsPropertyConversionService _botCommandPropertyConversionService;
        private readonly BotCommandsRequestValueGetterService _botCommandsRequestValueGetterService;

        public BotCommandsParsingService(BotCommandsPropertyConversionService botCommandPropertyConversionService, BotCommandsRequestValueGetterService botCommandsRequestValueGetterService)
        {
            this._botCommandPropertyConversionService = botCommandPropertyConversionService;
            this._botCommandsRequestValueGetterService = botCommandsRequestValueGetterService;
        }

        public async Task<IBotCommand> ParseRequestToCommand(Type commandType, DiscordRequest request, BotCommandTemplate template)
        {
            var result = await this.GetFilledInstance(commandType, template, (key, isList) => this._botCommandsRequestValueGetterService.GetValueByName(key, isList, request, template));
            return result;
        }

        public async Task<IBotCommand> ParseCustomTemplate(Type commandType, BotCommandTemplate template, Regex customTemplate, string input)
        {
            var match = customTemplate.Match(input);
            if (!this.CustomTemplateIsValid(match, template))
            {
                Log.Warning("Custom template {customTemplate} is not valid for {commandName}", customTemplate, template.CommandName);
                return null;
            }
            var result = await this.GetFilledInstance(commandType, template, (key, isList) => this._botCommandsRequestValueGetterService.GetValueByNameFromCustomCommand(key, isList, template, match));
            return result;
        }

        private async Task<IBotCommand> GetFilledInstance(Type commandType, BotCommandTemplate template, Func<string, bool, object> getValueByName)
        {
            var instance = Activator.CreateInstance(commandType);
            foreach (var property in commandType.GetProperties())
            {
                var propertyType = (template.Properties.FirstOrDefault(x => x.Name == property.Name)?.Type).Value;
                var isList = propertyType == BotCommandPropertyType.List;         
                var value = getValueByName.Invoke(property.Name, isList);
                var isPropertyOptional = (template.Properties.FirstOrDefault(x => x.Name == property.Name)?.IsOptional).Value;
                if (value == null)
                { 
                    if (!isPropertyOptional)
                    {
                        throw new NotEnoughArgumentsException();
                    }
                    continue;
                }
                if (isList && value is List<string> valueList)
                {
                    var IsEmptyList = !valueList.Any() || string.IsNullOrWhiteSpace(valueList.First());
                    if (IsEmptyList && !isPropertyOptional)
                    {
                        throw new NotEnoughArgumentsException();
                    }
                    property.SetValue(instance, value);
                    continue;
                }
                if (value is string valueString)
                {
                    if (string.IsNullOrWhiteSpace(valueString) && !isPropertyOptional)
                    {
                        throw new NotEnoughArgumentsException();
                    }
                    var convertedType = await this._botCommandPropertyConversionService.ConvertType(valueString, propertyType);
                    property.SetValue(instance, convertedType);
                }
            }
            return (IBotCommand) instance;
        }

        private bool CustomTemplateIsValid(Match match, BotCommandTemplate template)
        {
            if (!match.Success)
            {
                return false;
            }
            var requiredProperties = template.Properties.Where(x => !x.IsOptional).ToList();
            if (match.Groups.Count - 1 < requiredProperties.Count)
            {
                return false;
            }
            if (requiredProperties.Any(x => !match.Groups.ContainsKey(x.Name)))
            {
                return false;
            }
            return true;
        }
    }
}
