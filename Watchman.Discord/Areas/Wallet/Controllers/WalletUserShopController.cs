using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;

using Watchman.Discord.Areas.Wallet.BotCommands.Shops;

namespace Watchman.Discord.Areas.Wallet.Controllers
{
    /*
     * users should have possiblity to have they own shops where they can sell some services
     * as services I mean help in some things, for example do a code review or do a homework (second is more realistic)
     */
    public class WalletUserShopController : IController
    {
        public void ShowServerShop(ShowUserShopCommand command, Contexts contexts)
        {
        }

        public void AddProductToShop(AddProductToUserShopCommand command, Contexts contexts)
        {
        }

        public void RemoveProductFromShop(RemoveProductFromUserShopCommand command, Contexts contexts)
        {
        }

        public void UpdateProductInShop(UpdateProductInUserShopCommand command, Contexts contexts)
        {
        }

        public void BuyProduct(BuyUserProductCommand command, Contexts contexts)
        {
        }
    }
}
