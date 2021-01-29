using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Watchman.Cqrs;

namespace Watchman.DomainModel.Wallet.Queries
{
    public class GetUserWalletQuery : IQuery<GetUserWalletQueryResult>
    {
        public ulong ServerId { get; private set; }
        public ulong UserId { get; private set; }

        public GetUserWalletQuery(ulong serverId, ulong userId)
        {
            this.ServerId = serverId;
            this.UserId = userId;
        }
    }
}
