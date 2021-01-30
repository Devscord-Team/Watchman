
using Watchman.Cqrs;

namespace Watchman.DomainModel.Wallet.Commands
{
    public class RefreshWalletValueCommand : ICommand
    {
        public ulong ServerId { get; }
        public ulong UserId { get; }
        public RefreshWalletValueCommand(ulong serverId, ulong userId)
        {
            this.ServerId = serverId;
            this.UserId = userId;
        }
    }
}
