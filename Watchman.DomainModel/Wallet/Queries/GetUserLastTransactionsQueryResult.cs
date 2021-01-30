using System.Collections.Generic;

using Watchman.Cqrs;
using Watchman.DomainModel.Wallet.ValueObjects;

namespace Watchman.DomainModel.Wallet.Queries
{
    public class GetUserLastTransactionsQueryResult : IQueryResult
    {
        public IEnumerable<WalletTransaction> Transactions { get; }

        public GetUserLastTransactionsQueryResult(IEnumerable<WalletTransaction> transactions)
        {
            this.Transactions = transactions;
        }
    }
}
