using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;

using Watchman.Discord.Areas.Wallet.BotCommands.Shops;

namespace Watchman.Discord.Areas.Wallet.Controllers
{
    /*
     * because of every server have limited budget and small income, servers should have possiblity to earn money from users
     * as products I mean sending messages to administrators with requsts - every server should have channel with "requests storage" and selected moderators should get private messages with notifications
     * so that everything should be possible, from buying announcements on server to things like t-shirts or pizza (if server want to buy that for most active members)
     * or automated actions like 
     */
    public class WalletServerShopController : IController
    {
        public void ShowServerShop(ShowServerShopCommand command, Contexts contexts)
        {
        }

        public void AddProductToShop(AddProductToServerShopCommand command, Contexts contexts)
        {
        }

        public void RemoveProductFromShop(RemoveProductFromServerShopCommand command, Contexts contexts)
        {
        }

        public void UpdateProductInShop(UpdateProductInServerShopCommand command, Contexts contexts)
        {
        }

        public void BuyServerProduct(BuyServerProductCommand command, Contexts contexts)
        {
        }
    }
}
