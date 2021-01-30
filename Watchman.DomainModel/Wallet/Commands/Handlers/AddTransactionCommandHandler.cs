using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Watchman.Cqrs;
using Watchman.DomainModel.Wallet.Services;
using Watchman.DomainModel.Wallet.ValueObjects;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Wallet.Commands.Handlers
{
    public class AddTransactionCommandHandler : ICommandHandler<AddTransactionCommand>
    {
        private readonly ISessionFactory sessionFactory;
        private readonly RefreshingWalletService refreshingWalletService;

        public AddTransactionCommandHandler(ISessionFactory sessionFactory, RefreshingWalletService refreshingWalletService)
        {
            this.sessionFactory = sessionFactory;
            this.refreshingWalletService = refreshingWalletService;
        }

        public Task HandleAsync(AddTransactionCommand command)
        {
            var transaction = new WalletTransaction(command.OnServerId, command.FromUserId, command.ToUserId, command.Value, command.Title, command.Description, fromUserWalletValueIsCalculated: true);

            using var session = this.sessionFactory.CreateMongo();
            Task.WaitAll(
                this.refreshingWalletService.RefreshWallet(session, command.OnServerId, command.FromUserId, transaction), 
                this.refreshingWalletService.RefreshWallet(session, command.OnServerId, command.ToUserId, transaction));
            return session.AddAsync(transaction);
        }

        
    }
}
