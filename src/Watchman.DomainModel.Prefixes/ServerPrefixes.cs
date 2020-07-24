using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.DomainModel.Commons.Exceptions;
using Watchman.DomainModel.Prefixes.Events;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Prefixes
{
    public class ServerPrefixes : Entity, IAggregateRoot
    {
        private ISet<string> _prefixes = new HashSet<string>();

        public ulong ServerId { get; private set; }
        public IEnumerable<string> Prefixes { get => _prefixes; protected set => _prefixes = new HashSet<string>(value); }

        public ServerPrefixes(ulong serverId)
        {
            this.ServerId = serverId;
        }

        public async Task AddPrefix(string prefix)
        {
            if (this._prefixes.Any(x => x == prefix))
            {
                throw new AlreadyExistsException();
            }
            this._prefixes.Add(prefix);
            this.Update();
            await new PrefixAddedToServerEvent(this.ServerId, prefix).Publish();
        }

        public async Task DeletePrefix(string prefix)
        {
            if (!this._prefixes.Any(x => x == prefix))
            {
                return;
            }
            if (this._prefixes.Count <= 1)
            {
                return;
            }
            this._prefixes.Remove(prefix);
            this.Update();
            await new PrefixRemovedFromServerEvent(this.ServerId, prefix).Publish();
        }
    }
}
