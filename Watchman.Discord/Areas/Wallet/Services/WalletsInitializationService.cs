using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Watchman.Cqrs;
using Watchman.DomainModel.Wallet.Commands;
using Watchman.DomainModel.Wallet.Queries;

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
            var walletInRepositoryQuery = new GetUserWalletQuery(serverId, userId);
            var walletInRepository = this.queryBus.Execute(walletInRepositoryQuery).Wallet;
            if (walletInRepository != null)
            {
                return Task.CompletedTask;
            }
            var initializeWalletForUser = new InitializeWalletForUserCommand(serverId, userId);
            return this.commandBus.ExecuteAsync(initializeWalletForUser);
        }
    }
}
