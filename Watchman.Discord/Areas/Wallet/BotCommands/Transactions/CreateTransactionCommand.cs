using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watchman.Discord.Areas.Wallet.BotCommands.Transactions
{
    public class CreateTransactionCommand : IBotCommand
    {
        [UserMention]
        public ulong ToUser { get; set; }
        [Number]
        public uint Value { get; set; }
        [Text]
        public string Title { get; set; }
        [Optional]
        [Text]
        public string Description { get; set; }
    }
}
