using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Serilog;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Devscord.DiscordFramework.Framework.Commands.Properties;

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

        public IBotCommand ParseRequestToCommand(Type commandType, DiscordRequest request, BotCommandTemplate template)
        {
            var result = this.GetFilledInstance(commandType, template, (key, isList) => this._botCommandsRequestValueGetterService.GetValueByName(key, isList, request, template));
            return result;
        }

        public IBotCommand ParseCustomTemplate(Type commandType, BotCommandTemplate template, Regex customTemplate, string input)
        {
            var match = customTemplate.Match(input);
            if (!this.CustomTemplateIsValid(match, template))
            {
                Log.Warning("Custom template {customTemplate} is not valid for {commandName}", customTemplate, template.CommandName);
                return null;
            }
            var result = this.GetFilledInstance(commandType, template, (key, isList) => this._botCommandsRequestValueGetterService.GetValueByNameFromCustomCommand(key, isList, template, match));
            return result;
        }

        private IBotCommand GetFilledInstance(Type commandType, BotCommandTemplate template, Func<string, bool, object> getValueByName)
        {
            var instance = Activator.CreateInstance(commandType);
            foreach (var property in commandType.GetProperties())
            {
                var propertyType = template.Properties.FirstOrDefault(x => x.Name == property.Name)?.Type;
                var isList = propertyType == BotCommandPropertyType.List;
                var value = getValueByName.Invoke(property.Name, isList);
                if (value == null)
                {
                    continue;
                }
                if (isList)
                {
                    property.SetValue(instance, value);
                    continue;
                }
                if (value is string valueString)
                {
                    var convertedType = this._botCommandPropertyConversionService.ConvertType(valueString, propertyType.Value);
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
