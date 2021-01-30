
using Watchman.Cqrs;

namespace Watchman.DomainModel.Wallet.Queries
{
    public class GetUserLastTransactionsQuery : IQuery<GetUserLastTransactionsQueryResult>
    {
        public ulong ServerId { get; private set; }
        public ulong UserId { get; private set; }
        public int Quantity { get; private set; }

        public GetUserLastTransactionsQuery(ulong serverId, ulong userId, uint quantity)
        {
            this.ServerId = serverId;
            this.UserId = userId;
            this.Quantity = (int)quantity;
        }
    }
}
