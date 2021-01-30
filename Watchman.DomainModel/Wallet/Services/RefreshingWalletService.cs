using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Watchman.DomainModel.Wallet.ValueObjects;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Wallet.Services
{
    public class RefreshingWalletService
    {
        public Task RefreshWallet(ISession session, ulong serverId, ulong userId, WalletTransaction newTransaction = null)
        {
            if (userId == WalletTransaction.DEVSCORD_TEAM_TRANSACTION_USER_ID)
            {
                return Task.CompletedTask;
            }

            var wallet = session.Get<Wallet>().FirstOrDefault(x => x.ServerId == serverId && x.UserId == userId);

            var timestamp = DateTime.UtcNow;
            var fromUserTransactions = this.GetUserTransactions(session, serverId, userId).ToList();

            if(newTransaction != null)
            {
                fromUserTransactions.Add(newTransaction); //IMPORTANT
            }
            
            this.CalculateWallet(wallet, fromUserTransactions);

            var transactionsCreatedInMeantime = this.GetUserTransactions(session, serverId, userId).Where(x => x.CreatedAt >= timestamp);
            if (transactionsCreatedInMeantime.Any()) //double check -> maybe there should be loop, but double should be enough
            {
                fromUserTransactions.AddRange(transactionsCreatedInMeantime);
                this.CalculateWallet(wallet, fromUserTransactions);
            }

            return session.UpdateAsync(wallet);
        }

        private void CalculateWallet(Wallet wallet, IEnumerable<WalletTransaction> transactions)
        {
            if (transactions.Any())
            {
                wallet.FillTransactions(transactions, calculate: true);
            }
        }

        private IEnumerable<WalletTransaction> GetUserTransactions(ISession session, ulong serverId, ulong userId)
        {
            return session.Get<WalletTransaction>().Where(x => x.OnServerId == serverId && x.FromUserId == userId || x.ToUserId == userId);
        }
    }
}
