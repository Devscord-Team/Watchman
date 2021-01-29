using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Watchman.Cqrs;
using Watchman.Discord.Areas.Wallet.BotCommands.Transactions;
using Watchman.DomainModel.Wallet.Commands;

namespace Watchman.Discord.Areas.Wallet.Controllers
{
    /*
     * transactions in relation user-user and user-server
     * there should be also transaction-tax (money for server, tax should be possible to set by server in WalletServerController)
     * there should be also possibility to undo transaction -> create new transaction but reverted, for example when user A bought some service from user B, but user B cannot do that
     */
    public class WalletTransactionsController : IController 
    {
        private readonly ICommandBus commandBus;
        private readonly MessagesServiceFactory messagesServiceFactory;

        public WalletTransactionsController(ICommandBus commandBus, MessagesServiceFactory messagesServiceFactory)
        {
            this.commandBus = commandBus;
            this.messagesServiceFactory = messagesServiceFactory;
        }

        public async Task CreateTransaction(CreateTransactionCommand command, Contexts contexts)
        {
            var createTransactionCommand = new AddTransactionCommand(contexts.Server.Id, contexts.User.Id, command.ToUser, command.Value, command.Title, command.Description);
            await this.commandBus.ExecuteAsync(createTransactionCommand);
            var messagesService = this.messagesServiceFactory.Create(contexts);
            await messagesService.SendEmbedMessage("Transaction", $"Transaction has been created", new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("On server", contexts.Server.Name),
                new KeyValuePair<string, string>("From user", contexts.User.Id.GetUserMention()),
                new KeyValuePair<string, string>("To user", command.ToUser.GetUserMention()),
                new KeyValuePair<string, string>("Title", command.Title),
                new KeyValuePair<string, string>("Value", command.Value.ToString())
            });
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
