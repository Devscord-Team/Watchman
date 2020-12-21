using System.Collections.Generic;

namespace Devscord.DiscordFramework.Services.Models
{
    public readonly struct BotCommandInformation
    {
        public string Name { get; }
        public string AreaName { get; }
        public IEnumerable<BotArgumentInformation> BotCommandArgumentInformations { get; }

        public BotCommandInformation(string name, string areaName, IEnumerable<BotArgumentInformation> botCommandArgumentInformations)
        {
            this.Name = name;
            this.AreaName = areaName;
            this.BotCommandArgumentInformations = botCommandArgumentInformations;
        }
    }
}
