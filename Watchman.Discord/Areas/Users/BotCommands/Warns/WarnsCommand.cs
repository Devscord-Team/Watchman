using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Users.BotCommands.Warns
{
    public class WarnsCommand : IBotCommand
    {
        [Optional]
        public string User { get; set; }

        [Bool]
        public bool All { get; set; }
    }
}
