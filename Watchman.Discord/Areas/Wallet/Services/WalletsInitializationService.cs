using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Watchman.Cqrs;

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
            return Task.CompletedTask;
        }
    }
}
