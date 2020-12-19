using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Wallet.ValueObjects
{
    public class Transaction : Entity
    {
        private bool isValid;

        public ulong OnServerId { get; private set; }
        public ulong FromUserId { get; private set; }
        public ulong ToUserId { get; private set; }
        public int Value { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }

        public Transaction(ulong onServerId, ulong fromUserId, ulong toUserId, int value, string title, string description)
        {
            this.OnServerId = onServerId;
            this.FromUserId = fromUserId;
            this.ToUserId = toUserId;
            this.Value = value;
            this.Title = title;
            this.Description = description;
        }

        public int GetValue() //Get value always by this method, never by value property
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
            if(this.FromUserId == 0 || this.ToUserId == 0)
            {
                this.isValid = false;
                return this.isValid;
            }
            if(this.Value == 0)
            {
                this.isValid = false;
                return this.isValid;
            }
            if(string.IsNullOrWhiteSpace(this.Title))
            {
                this.isValid = false;
                return this.isValid;
            }
            if(string.IsNullOrWhiteSpace(this.Description))
            {
                this.isValid = false;
                return this.isValid;
            }

            this.isValid = true;
            return this.isValid;
        }
    }
}
