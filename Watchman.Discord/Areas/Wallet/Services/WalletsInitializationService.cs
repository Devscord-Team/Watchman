using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Watchman.Cqrs;
using Watchman.DomainModel.Wallet.Commands;
using Watchman.DomainModel.Wallet.Queries;
using Watchman.DomainModel.Wallet.ValueObjects;

namespace Watchman.Discord.Areas.Wallet.Services
{
    public class WalletsInitializationService
    {
        private readonly IQueryBus queryBus;
        private readonly ICommandBus commandBus;

        public WalletsInitializationService(IQueryBus queryBus, ICommandBus commandBus)
        {
            this.queryBus = queryBus;
            this.commandBus = commandBus;
        }

        public Task TryCreateServerWalletForUser(ulong serverId, ulong userId)
        {
            if (userId == WalletTransaction.DEVSCORD_TEAM_TRANSACTION_USER_ID)
            {
                return Task.CompletedTask;
            }
            var walletInRepositoryQuery = new GetUserWalletQuery(serverId, userId);
            var walletInRepository = this.queryBus.Execute(walletInRepositoryQuery).Wallet;
            if (walletInRepository != null)
            {
                return Task.CompletedTask;
            }
            var initializeWalletForUser = new InitializeWalletForUserCommand(serverId, userId);
            return this.commandBus.ExecuteAsync(initializeWalletForUser);
        }

        public async Task TryCreateServerWalletForServer(ulong serverId)
        {
            var walletInRepositoryQuery = new GetUserWalletQuery(serverId, 0);
            var walletInRepository = this.queryBus.Execute(walletInRepositoryQuery).Wallet;
            if (walletInRepository != null)
            {
                return;
            }
            var initializeWalletForUser = new InitializeWalletForUserCommand(serverId, 0);
            await this.commandBus.ExecuteAsync(initializeWalletForUser);

            var initialMoneyCommand = new AddTransactionCommand(serverId, WalletTransaction.DEVSCORD_TEAM_TRANSACTION_USER_ID, 0, 1000, "Initial transaction", string.Empty);
            await this.commandBus.ExecuteAsync(initialMoneyCommand);
        }
    }
}
