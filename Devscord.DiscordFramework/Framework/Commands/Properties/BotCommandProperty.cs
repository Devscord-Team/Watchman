namespace Devscord.DiscordFramework.Framework.Commands.Properties
{
    public class BotCommandProperty
    {
        public string Name { get; private set; }
        public BotCommandPropertyType Type { get; private set; }
        public bool IsOptional { get; private set; }

        public BotCommandProperty(string name, BotCommandPropertyType type, bool isOptional)
        {
            Name = name;
            Type = type;
            IsOptional = isOptional;
        }
    }
}
