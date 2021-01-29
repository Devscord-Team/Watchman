
using Watchman.Cqrs;

namespace Watchman.DomainModel.Wallet.Commands
{
    public class AddTransactionCommand : ICommand
    {
        public ulong OnServerId { get; }
        public ulong FromUserId { get; }
        public ulong ToUserId { get; }
        public uint Value { get; }
        public string Title { get; }
        public string Description { get; }

        public AddTransactionCommand(ulong onServerId, ulong fromUserId, ulong toUserId, uint value, string title, string description)
        {
            this.OnServerId = onServerId;
            this.FromUserId = fromUserId;
            this.ToUserId = toUserId;
            this.Value = value;
            this.Title = title;
            this.Description = description;
        }
    }
}
