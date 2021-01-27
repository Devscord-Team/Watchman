﻿using MongoDB.Bson.Serialization.Attributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Wallet
{
    public class Wallet : Entity, IAggregateRoot
    {
        public ulong UserId { get; private set; }
        [BsonIgnore]
        public IEnumerable<ValueObjects.WalletTransaction> Transactions { get; private set; }
        public uint Value { get; private set; } //calculate after transaction and user command

        public Wallet(ulong userId)
        {
            this.UserId = userId;
            this.Transactions = new List<ValueObjects.WalletTransaction>();
            this.Value = 0;
        }

        public void FillTransactions(IEnumerable<ValueObjects.WalletTransaction> transactions)
        {
            if(transactions.Any(x => x.FromUserId != this.UserId && x.ToUserId != this.UserId))
            {
                throw new ArgumentException("System is trying to put transactions to wrong wallet");
            }
            if(transactions != null)
            {
                throw new ArgumentException("System is trying to put transactions to wallet that already has transactions");
            }
            this.Transactions = transactions;
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
                if(transaction.FromUserId == this.UserId)
                {
                    value -= transaction.Value;
                }
                else 
                {
                    value += transaction.Value;
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
