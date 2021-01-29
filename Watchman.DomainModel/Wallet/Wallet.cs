using MongoDB.Bson.Serialization.Attributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Watchman.DomainModel.Wallet.ValueObjects;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Wallet
{
    public class Wallet : Entity, IAggregateRoot
    {
        public ulong ServerId { get; private set; }
        public ulong UserId { get; private set; } //if 0 -> this is server wallet
        [BsonIgnore]
        public IEnumerable<WalletTransaction> Transactions { get; private set; }
        public uint Value { get; private set; } //calculate after transaction and user command

        public Wallet(ulong serverId, ulong userId)
        {
            this.ServerId = serverId;
            this.UserId = userId;
            this.Transactions = new List<WalletTransaction>();
            this.Value = 0;
        }

        public void FillTransactions(IEnumerable<WalletTransaction> transactions, bool calculate = true)
        {
            if(transactions.Any(x => x.FromUserId != this.UserId && x.ToUserId != this.UserId))
            {
                throw new ArgumentException("System is trying to put transactions to wrong wallet");
            }
            this.Transactions = transactions;
            if(calculate)
            {
                this.CalculateValue();
            }
        }

        public void CalculateValue()
        {
            if(this.Transactions == null || !this.Transactions.Any())
            {
                throw new ArgumentException("Fill transactions before calculate wallet value");
            }
            long value = 0;
            foreach (var transaction in this.Transactions)
            {
                if(!transaction.IsValid)
                {
                    transaction.Validate();
                }
                if(transaction.FromUserId == this.UserId)
                {
                    value -= transaction.GetValue();
                }
                else 
                {
                    value += transaction.GetValue();
                }
            }
            if(value < 0)
            {
                throw new Exception("Wallet value cannot be negative");
            }
            if(this.Value == value)
            {
                return;
            }
            this.Value = Convert.ToUInt32(value);
            this.Update();
        }
    }
}
