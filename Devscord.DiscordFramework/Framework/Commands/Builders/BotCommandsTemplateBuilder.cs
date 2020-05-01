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
            var properties = GetBotCommandProperties(commandType);
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
            var attributes = commandProperty.GetCustomAttributes(typeof(CommandPropertyAttribute), inherit: true).Select(x => x as CommandPropertyAttribute);
            var attribute = attributes.FirstOrDefault(x => !(x is Optional)) ?? new SingleWord();
            var type = (BotCommandPropertyType)Enum.Parse(typeof(BotCommandPropertyType), attribute.GetType().Name);
            var isOptional = attributes.Any(x => x is Optional);
            return new BotCommandProperty(name, type, isOptional);
        }
    }
}
