﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Wallet.ValueObjects
{
    //todo - block transactions mechanism when in last time was any exception (wallet calculation)
    //we don't want to do any transactions when our code isn't correct
    public class WalletTransaction : Entity
    {
        public static ulong DEVSCORD_TEAM_TRANSACTION_USER_ID = 636274997786312723; //Watchman UserId

        private bool isValid;

        public ulong OnServerId { get; private set; }
        public ulong FromUserId { get; private set; }
        public ulong ToUserId { get; private set; }
        public uint Value { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }

        public WalletTransaction(ulong onServerId, ulong fromUserId, ulong toUserId, uint value, string title, string description, bool fromUserWalletValueIsCalculated)
        {
            if(!fromUserWalletValueIsCalculated)
            {
                throw new ArgumentException("User wallet must be calculated");
            }
            this.OnServerId = onServerId;
            this.FromUserId = fromUserId;
            this.ToUserId = toUserId;
            this.Value = value;
            this.Title = title;
            this.Description = description;
            if(!this.Validate())
            {
                throw new ArgumentException("System is trying to create transaction with invalid values");
            }
        }

        public uint GetValue() //Get value always by this method, never by value property
        {
            if (!this.isValid)
            {
                throw new ArgumentException($"System is trying to get value from invalid transaction - id: {this.Id} | createdAt: {this.CreatedAt}");
            }
            return this.Value;
        }

        public bool Validate()
        {
            if(this.FromUserId == this.ToUserId)
            {
                this.isValid = false;
                return this.isValid;
            }
            if (this.Value == 0)
            {
                this.isValid = false;
                return this.isValid;
            }
            if (this.Value < 0)
            {
                this.isValid = false;
                return this.isValid;
            }
            if (string.IsNullOrWhiteSpace(this.Title))
            {
                this.isValid = false;
                return this.isValid;
            }

            this.isValid = true;
            return this.isValid;
        }

        public bool IsValid => this.isValid;
    }
}
