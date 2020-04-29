namespace Devscord.DiscordFramework.Framework.Commands.Properties
{
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
}
