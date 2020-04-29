using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Framework.Commands.Properties;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Devscord.DiscordFramework.Framework.Commands
{
    public class BotCommandsService
    {
        private readonly Regex exTime = new Regex(@"\d+(h|m|s)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Regex exMention = new Regex(@"<@&?\d+>", RegexOptions.Compiled);
        private readonly BotCommandPropertyConversionService botCommandPropertyConversionService;

        public BotCommandsService(BotCommandPropertyConversionService botCommandPropertyConversionService)
        {
            this.botCommandPropertyConversionService = botCommandPropertyConversionService;
        }

        public string RenderTextTemplate(BotCommandTemplate template)
        {
            var output = new StringBuilder();
            output.Append($"{{{{prefix}}}}[[{template.CommandName}]]");
            foreach (var commandProperty in template.Properties)
            {
                output.Append($" {{{{prefix}}}}[[{commandProperty.Name}]] (({Enum.GetName(typeof(BotCommandPropertyType), commandProperty.Type)}))");
            }
            return output.ToString();
        }

        public BotCommandTemplate GetCommandTemplate(Type commandType)
        {
            var properties = this.GetBotCommandProperties(commandType);
            var template = new BotCommandTemplate(commandType.Name, properties);
            return template;
        }

        public bool IsMatchedWithCommand(DiscordRequest request, BotCommandTemplate template)
        {
            if (!request.IsCommandForBot)
            {
                return false;
            }
            if (request.Name.ToLowerInvariant() != template.NormalizedCommandName)
            {
                return false;
            }
            if(!this.CompareArgumentsToProperties(request.Arguments.ToList(), template.Properties.ToList()))
            {
                return false;
            }

            return true;
        }

        public T ParseRequestToCommand<T>(DiscordRequest request, BotCommandTemplate template) where T : IBotCommand
        {
            return (T) this.ParseRequestToCommand(typeof(T), request, template);
        }

        public IBotCommand ParseRequestToCommand(Type commandType, DiscordRequest request, BotCommandTemplate template)
        {
            var instance = Activator.CreateInstance(commandType);
            foreach (var property in commandType.GetProperties())
            {
                var value = request.Arguments.First(x => x.Name.ToLowerInvariant() == property.Name.ToLowerInvariant()).Value;
                var propertyType = template.Properties.First(x => x.Name == property.Name).Type;
                var convertedType = this.botCommandPropertyConversionService.ConvertType(value, propertyType);
                property.SetValue(instance, convertedType);
            }
            return (IBotCommand) instance;
        }

        private IEnumerable<BotCommandProperty> GetBotCommandProperties(Type commandType)
        {
            foreach (var commandProperty in commandType.GetProperties())
            {
                var name = commandProperty.Name;
                var attribute = commandProperty.GetCustomAttributes(typeof(CommandPropertyAttribute), true)
                    .Select(x => x as CommandPropertyAttribute)
                    .FirstOrDefault() ?? new SingleWord();
                var type = (BotCommandPropertyType) Enum.Parse(typeof(BotCommandPropertyType), attribute.GetType().Name);
                yield return new BotCommandProperty(name, type);
            }
        }

        private bool CompareArgumentsToProperties(List<DiscordRequestArgument> arguments, List<BotCommandProperty> properties)
        {
            if (arguments.Count != properties.Count)
            {
                return false;
            }
            foreach (var argument in arguments)
            {
                var anyIsmatched = properties.Any(property => argument.Name.ToLowerInvariant() == property.Name.ToLowerInvariant() && IsMatchedPropertyType(argument.Value, property.Type));
                if(!anyIsmatched)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsMatchedPropertyType(string value, BotCommandPropertyType type)
        {
            if(type == BotCommandPropertyType.Number && !int.TryParse(value, out _))
            {
                return false;
            }
            else if (type == BotCommandPropertyType.Time && !exTime.IsMatch(value))
            {
                return false;
            }
            else if (type == BotCommandPropertyType.UserMention || type == BotCommandPropertyType.ChannelMention && !exMention.IsMatch(value))
            {
                return false;
            }
            else if (type == BotCommandPropertyType.SingleWord && value.Contains(' '))
            {
                return false;
            }

            return true;
        }

        
    }
}
