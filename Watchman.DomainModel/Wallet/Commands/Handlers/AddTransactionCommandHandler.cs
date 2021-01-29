using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Watchman.Cqrs;
using Watchman.DomainModel.Wallet.ValueObjects;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Wallet.Commands.Handlers
{
    public class AddTransactionCommandHandler : ICommandHandler<AddTransactionCommand>
    {
        private readonly ISessionFactory sessionFactory;

        public AddTransactionCommandHandler(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public Task HandleAsync(AddTransactionCommand command)
        {
            var transaction = new WalletTransaction(command.OnServerId, command.FromUserId, command.ToUserId, command.Value, command.Title, command.Description, fromUserWalletValueIsCalculated: true);

            using var session = this.sessionFactory.CreateMongo();
            Task.WaitAll(
                this.RefreshWallet(session, command.OnServerId, command.FromUserId, transaction), 
                this.RefreshWallet(session, command.OnServerId, command.ToUserId, transaction));
            return session.AddAsync(transaction);
        }

        private Task RefreshWallet(ISession session, ulong serverId, ulong userId, WalletTransaction newTransaction)
        {
            var wallet = session.Get<Wallet>().FirstOrDefault(x => x.ServerId == serverId && x.UserId == userId);

            var timestamp = DateTime.UtcNow;
            var fromUserTransactions = this.GetUserTransactions(session, serverId, userId).ToList();

            fromUserTransactions.Add(newTransaction); //IMPORTANT

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
                wallet.FillTransactions(transactions);
                wallet.CalculateValue();
            }
        }

        private IEnumerable<WalletTransaction> GetUserTransactions(ISession session, ulong serverId, ulong userId)
        {
            return session.Get<WalletTransaction>().Where(x => x.OnServerId == serverId && x.FromUserId == userId || x.ToUserId == userId);
        }
    }
}
