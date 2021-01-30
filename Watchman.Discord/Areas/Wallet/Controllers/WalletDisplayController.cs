using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Watchman.Cqrs;
using Watchman.Discord.Areas.Wallet.BotCommands.Display;
using Watchman.DomainModel.Wallet.Queries;

namespace Watchman.Discord.Areas.Wallet.Controllers
{
    /*
     * display user wallet, server wallet or server-section wallet
     */
    public class WalletDisplayController : IController
    {
        private readonly IQueryBus queryBus;
        private readonly MessagesServiceFactory messagesServiceFactory;

        public WalletDisplayController(IQueryBus queryBus, MessagesServiceFactory messagesServiceFactory)
        {
            this.queryBus = queryBus;
            this.messagesServiceFactory = messagesServiceFactory;
        }

        public Task ShowWalet(ShowWalletCommand command, Contexts contexts)
        {
            var userId = contexts.User.Id;
            if(command.User > 0)
            {
                userId = command.User;
            }
            var wallet = this.queryBus.Execute(new GetUserWalletQuery(contexts.Server.Id, userId)).Wallet;
            var messagesService = this.messagesServiceFactory.Create(contexts);
            return messagesService.SendEmbedMessage("Wallet", $"Wallet of user: {userId.GetUserMention()}", new List<KeyValuePair<string, string>> 
            { 
                new KeyValuePair<string, string>("Value", wallet.Value.ToString()) 
            });
        }

        public Task ShowLastTransactions(ShowLastTransactionsCommand command, Contexts contexts)
        {
            var userId = contexts.User.Id;
            if (command.User > 0)
            {
                userId = command.User;
            }
            var transactionsQuery = new GetUserLastTransactionsQuery(contexts.Server.Id, userId, command.Quantity);
            var transactions = this.queryBus.Execute(transactionsQuery).Transactions;
            var messagesService = this.messagesServiceFactory.Create(contexts);
            return messagesService.SendEmbedMessage("Transactions", $"Last {command.Quantity} transactions of user: {userId.GetUserMention()}", 
                transactions.Select(transaction => 
                {
                    var stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine($"{transaction.Title}");
                    if(!string.IsNullOrWhiteSpace(transaction.Description))
                    {
                        stringBuilder.AppendLine($"Description:");
                        stringBuilder.AppendLine(transaction.Description);
                    }
                    stringBuilder.AppendLine($"====");
                    stringBuilder.AppendLine($"From user: {transaction.FromUserId.GetUserMention()}");
                    stringBuilder.AppendLine($"To user: {transaction.ToUserId.GetUserMention()}");
                    stringBuilder.AppendLine($"====");
                    stringBuilder.AppendLine($"Value: {transaction.Value}");
                    stringBuilder.AppendLine($"Created at: {transaction.CreatedAt}");

                    return new KeyValuePair<string, string>(transaction.Title, stringBuilder.ToString());
                }));
        }

        public void ShowServerSectionWallet(ShowServerSectionWalletCommand command, Contexts contexts)
        {

        }

        public void ShowServerWallet(ShowServerWalletCommand command, Contexts contexts)
        {

        }

        public void ShowServerWalletSectionConfiguration(ShowServerWalletSectionConfigurationCommand command, Contexts contexts)
        {
        }

        public void ShowServerWalletConfiguration(ShowServerWalletConfigurationCommand command, Contexts contexts)
        {
        }

        public void ShowServerWalletConfigurationForUser(ShowServerWalletConfigurationForUserCommand command, Contexts contexts)
        {
        }
    }
}
