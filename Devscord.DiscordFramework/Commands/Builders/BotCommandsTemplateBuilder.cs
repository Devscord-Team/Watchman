﻿using Devscord.DiscordFramework.Commands;
using Devscord.DiscordFramework.Commands.Properties;
using Devscord.DiscordFramework.Commands.PropertyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Devscord.DiscordFramework.Commands.Builders
{
    public interface IBotCommandsTemplateBuilder
    {
        BotCommandTemplate GetCommandTemplate(Type commandType);
    }

    public class BotCommandsTemplateBuilder : IBotCommandsTemplateBuilder
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
            var attributes = commandProperty.GetCustomAttributes(typeof(CommandPropertyAttribute), inherit: true).Select(x => x as CommandPropertyAttribute).ToList();
            var attribute = attributes.FirstOrDefault(x => x.GetType() != typeof(Optional)) ?? new SingleWord();
            var type = (BotCommandPropertyType) Enum.Parse(typeof(BotCommandPropertyType), attribute.GetType().Name);
            var isOptional = attributes.Any(x => x is Optional);
            return new BotCommandProperty(name, type, isOptional);
        }
    }
}
