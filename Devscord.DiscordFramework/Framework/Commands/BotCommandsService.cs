using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devscord.DiscordFramework.Framework.Commands
{
    public class BotCommandsService
    {
        public BotCommandTemplate GetDefaultCommandTemplate(IBotCommand command)
        {
            var type = command.GetType();
            var properties = this.GetBotCommandProperties(type);
            var template = new BotCommandTemplate(type.Name, properties);
            return template;
        }

        private IEnumerable<BotCommandProperty> GetBotCommandProperties(Type commandType)
        {
            foreach (var commandProperty in commandType.GetProperties())
            {
                var name = commandProperty.Name;
                var attribute = commandProperty.GetCustomAttributes(typeof(CommandPropertyAttribute), true).Select(x => x as CommandPropertyAttribute).FirstOrDefault() ?? new SingleWord();
                var type = (BotCommandPropertyType) Enum.Parse(typeof(BotCommandPropertyType), attribute.GetType().Name);
                yield return new BotCommandProperty(name, type);
            }
        }
    }

    public class BotCommandTemplate
    {
        public string CommandName { get; private set; }
        public IEnumerable<BotCommandProperty> Properties { get; private set; }

        public BotCommandTemplate(string commandName, IEnumerable<BotCommandProperty> properties)
        {
            CommandName = commandName;
            Properties = properties;
        }
    }

    public class BotCommandProperty
    {
        public string Name { get; private set; }
        public BotCommandPropertyType Type { get; private set; }

        public BotCommandProperty(string name, BotCommandPropertyType type)
        {
            Name = name;
            Type = type;
        }
    }

    public enum BotCommandPropertyType
    {
        SingleWord = 0x1,
        Text = 0x2,
        Number = 0x4,
        Time = 0x8,
        UserMention = 0x16,
        ChannelMention = 0x32
    }

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class CommandPropertyAttribute : Attribute
    {
    }
    
    public class SingleWord : CommandPropertyAttribute
    {
    }

    public class Text : CommandPropertyAttribute
    {
    }

    public class Number : CommandPropertyAttribute
    {
    }

    public class Time : CommandPropertyAttribute
    {
    }

    public class UserMention : CommandPropertyAttribute
    {
    }

    public class ChannelMention : CommandPropertyAttribute
    {
    }
}
