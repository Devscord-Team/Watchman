using Devscord.DiscordFramework.Framework.Commands.Properties;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Devscord.DiscordFramework.Framework.Commands.Builders
{
    public class BotCommandsTemplateBuilder
    {
        public BotCommandTemplate GetCommandTemplate(Type commandType)
        {
            var properties = this.GetBotCommandProperties(commandType);
            var template = new BotCommandTemplate(commandType.Name, properties);
            return template;
        }

        private IEnumerable<BotCommandProperty> GetBotCommandProperties(Type commandType)
        {
            return commandType.GetProperties().Select(x => this.GetBotCommandProperty(x));
        }

        private BotCommandProperty GetBotCommandProperty(PropertyInfo commandProperty)
        {
            var name = commandProperty.Name;
            var attributes = commandProperty
                .GetCustomAttributes(typeof(CommandPropertyAttribute), inherit: true)
                .Select(x => x as CommandPropertyAttribute)
                .ToList();
            var attribute = attributes.FirstOrDefault(x => x.GetType() != typeof(Optional)) ?? new SingleWord();
            var generalType = (BotCommandPropertyType) Enum.Parse(typeof(BotCommandPropertyType), attribute.GetType().Name);
            var isOptional = attributes.Any(x => x is Optional);
            var actualType = commandProperty.PropertyType;
            return new BotCommandProperty(name, generalType, isOptional, actualType);
        }
    }
}
