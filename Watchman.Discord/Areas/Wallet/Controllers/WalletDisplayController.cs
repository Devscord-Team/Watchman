using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;

using System.Collections.Generic;
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

        public Task ShowMyWalet(ShowMyWalletCommand command, Contexts contexts)
        {
            var wallet = this.queryBus.Execute(new GetUserWalletQuery(contexts.Server.Id, contexts.User.Id)).Wallet;
            var messagesService = this.messagesServiceFactory.Create(contexts);
            return messagesService.SendEmbedMessage("Wallet", $"Wallet of user: {contexts.User.Mention}", new List<KeyValuePair<string, string>> 
            { 
                new KeyValuePair<string, string>("Value", wallet.Value.ToString()) 
            });
        }

        public void ShowDifferentUserWallet(ShowDifferentUserWalletCommand command, Contexts contexts)
        {

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
