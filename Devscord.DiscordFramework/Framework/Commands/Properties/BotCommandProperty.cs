using System;

namespace Devscord.DiscordFramework.Framework.Commands.Properties
{
    public class BotCommandProperty
    {
        public string Name { get; private set; }
        public BotCommandPropertyType GeneralType { get; private set; }
        public bool IsOptional { get; private set; }
        public Type ActualType { get; private set; }

        public BotCommandProperty(string name, BotCommandPropertyType generalType, bool isOptional, Type actualType)
        {
            this.Name = name;
            this.GeneralType = generalType;
            this.IsOptional = isOptional;
            this.ActualType = actualType;
        }
    }
}
