using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;

using Watchman.Discord.Areas.Wallet.BotCommands.Display;

namespace Watchman.Discord.Areas.Wallet.Controllers
{
    /*
     * display user wallet, server wallet or server-section wallet
     */
    public class WalletDisplayController : IController
    {
        public void ShowMyWalet(ShowMyWalletCommand command, Contexts contexts)
        {

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
