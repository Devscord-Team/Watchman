using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watchman.Discord.Areas.Wallet.BotCommands.Display
{
    public class ShowWalletCommand : IBotCommand
    {
        [Optional]
        [UserMention]
        public ulong User { get; set; }
    }
}
