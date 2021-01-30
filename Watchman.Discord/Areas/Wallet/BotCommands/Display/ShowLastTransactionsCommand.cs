using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Wallet.BotCommands.Display
{
    public class ShowLastTransactionsCommand : IBotCommand
    {
        [Optional]
        [UserMention]
        public ulong User { get; set; }
        [Number]
        public int Quantity { get; set; }
    }
}
