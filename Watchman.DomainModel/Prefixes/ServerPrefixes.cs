using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.ServerPrefixes
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

        public void AddPrefix(string prefix)
        {
            if (this._prefixes.Any(x => x == prefix))
            {
                return;
            }
            this._prefixes.Add(prefix);
            this.Update();
        }

        public void DeletePrefix(string prefix)
        {
            if (!this._prefixes.Any(x => x == prefix))
            {
                return;
            }
            if(this._prefixes.Count <= 1)
            {
                return;
            }
            this._prefixes.Remove(prefix);
            this.Update();
        }
    }
}
