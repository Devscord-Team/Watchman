using System;

namespace Devscord.DiscordFramework.Services.Models
{
    public class BotArgumentInformation
    {
        public string Name { get; }
        public Type ExpectedType { get; }
        public bool IsOptional { get; }

        public BotArgumentInformation(string name, Type expectedType, bool isOptional)
        {
            this.Name = name;
            this.ExpectedType = expectedType;
            this.IsOptional = isOptional;
        }
    }
}