using System.Collections.Generic;

namespace Devscord.DiscordFramework.Services.Models
{
    public class BotCommandInformation
    {
        public string Name { get; }
        public IEnumerable<BotArgumentInformation> BotCommandArgumentInformations { get; }

        public BotCommandInformation(string name, IEnumerable<BotArgumentInformation> botCommandArgumentInformations)
        {
            this.Name = name;
            this.BotCommandArgumentInformations = botCommandArgumentInformations;
        }
    }
}
