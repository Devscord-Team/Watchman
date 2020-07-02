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

<<<<<<< HEAD
        public BotCommandsParsingService(BotCommandsPropertyConversionService botCommandPropertyConversionService) => this.botCommandPropertyConversionService = botCommandPropertyConversionService;

        public IBotCommand ParseRequestToCommand(Type commandType, DiscordRequest request, BotCommandTemplate template)
        {
            var result = this.GetFilledInstance(commandType, template, x => request.Arguments.FirstOrDefault(a => a.Name.ToLowerInvariant() == x.ToLowerInvariant())?.Value);
=======
        public BotCommandsParsingService(BotCommandsPropertyConversionService botCommandPropertyConversionService, BotCommandsRequestValueGetterService botCommandsRequestValueGetterService)
        {
            this._botCommandPropertyConversionService = botCommandPropertyConversionService;
            this._botCommandsRequestValueGetterService = botCommandsRequestValueGetterService;
        }

        public IBotCommand ParseRequestToCommand(Type commandType, DiscordRequest request, BotCommandTemplate template)
        {
            var result = this.GetFilledInstance(commandType, template, (key, isList) => this._botCommandsRequestValueGetterService.GetValueByName(key, isList, request, template));
>>>>>>> master
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
<<<<<<< HEAD
            var result = this.GetFilledInstance(commandType, template, x => match.Groups.ContainsKey(x) ? match.Groups[x].Value : null);
=======
            var result = this.GetFilledInstance(commandType, template, (key, isList) => this._botCommandsRequestValueGetterService.GetValueByNameFromCustomCommand(key, isList, template, match));
>>>>>>> master
            return result;
        }

        private IBotCommand GetFilledInstance(Type commandType, BotCommandTemplate template, Func<string, bool, object> getValueByName)
        {
            var instance = Activator.CreateInstance(commandType);
            foreach (var property in commandType.GetProperties())
            {
<<<<<<< HEAD
                var value = getValueByName.Invoke(property.Name);
                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }
                var propertyType = template.Properties.First(x => x.Name == property.Name).Type;
                var convertedType = this.botCommandPropertyConversionService.ConvertType(value, propertyType);
                property.SetValue(instance, convertedType);
=======
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
                if (value is string valueString && !string.IsNullOrWhiteSpace(valueString))
                {
                    var convertedType = this._botCommandPropertyConversionService.ConvertType(valueString, propertyType.Value);
                    property.SetValue(instance, convertedType);
                }
>>>>>>> master
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
