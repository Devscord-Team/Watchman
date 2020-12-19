
using Watchman.Cqrs;

namespace Watchman.DomainModel.Wallet.Queries
{
    public class GetUserWalletQueryResult : IQueryResult
    {
        public Wallet Wallet { get; }

        public GetUserWalletQueryResult(Wallet wallet)
        {
            this.Wallet = wallet;
        }
    }
}
