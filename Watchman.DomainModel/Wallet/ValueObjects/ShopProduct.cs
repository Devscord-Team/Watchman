using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Watchman.DomainModel.Wallet.ValueObjects.Actions;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Wallet.ValueObjects
{
    public class ShopProduct : Entity
    {
        public string Name { get; private set; }
        public string Category { get; private set; } //display should be groupped by category, but in can be empty
        public uint Price { get; private set; }
        public ProductAction Action { get; private set; }
    }
}
