using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Watchman.Discord.Areas.Wallet.BotCommands.Transactions;

namespace Watchman.Discord.Areas.Wallet.Controllers
{
    /*
     * transactions in relation user-user and user-server
     * there should be also transaction-tax (money for server, tax should be possible to set by server in WalletServerController)
     * there should be also possibility to undo transaction -> create new transaction but reverted, for example when user A bought some service from user B, but user B cannot do that
     */
    public class WalletTransactionsController : IController 
    {
        public void CreateTransaction(CreateTransactionCommand command, Contexts contexts)
        {

        }

        /*
         * ask other side for allowment to undo transaction
         */
        public void UndoMyTransaction(UndoMyTransactionCommand command, Contexts contexts)
        {

        }

        /*
         * moderator should have possiblity to undo transaction between users
         */
        public void ForceUndoTransaction(ForceUndoTransactionCommand command, Contexts contexts)
        {

        }
    }
}
