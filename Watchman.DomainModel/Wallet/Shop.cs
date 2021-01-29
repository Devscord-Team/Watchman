using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Watchman.DomainModel.Wallet.ValueObjects;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Wallet
{
    /*
     * 2 aggregate roots in one aggregate, because in this case that is easier, but should be separated in future
     */
    public class Shop : Entity, IAggregateRoot
    {
        public ulong ServerId { get; private set; } //if this is != 0 -> this is server shop
        public ulong UserOwnerId { get; private set; }
        public IEnumerable<ShopProduct> Products { get; private set; }

        public Shop()
        {

        }
    }
}
