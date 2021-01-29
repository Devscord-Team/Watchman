using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Wallet.ValueObjects.Actions
{
    public abstract class ProductAction : Entity
    {
    }

    public class ProductActionSendPrivateMessage : ProductAction
    {
        public IEnumerable<ulong> UserIds { get; private set; }
        public string MessageTemplate { get; private set; } //template like in responses
        public IEnumerable<string> MessageTemplateVariables { get; private set; }
    }

    public class ProductActionSendMessageOnChannel : ProductAction
    {
        public ulong ServerId { get; private set; }
        public IEnumerable<ulong> ChannelIds { get; private set; }
        public string MessageTemplate { get; private set; } //template like in responses
        public IEnumerable<string> MessageTemplateVariables { get; private set; }
    }

    public class ProductActionAddRoleToUser : ProductAction
    {
        public ulong ServerId { get; private set; }
        public ulong RoleId { get; private set; }
    }

    public class ProductActionRemoveRoleFromUser : ProductAction
    {
        public ulong ServerId { get; private set; }
        public ulong RoleId { get; private set; }
    }
}
