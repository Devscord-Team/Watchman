using System;
using System.Collections.Generic;
using System.Text;

namespace Watchman.DomainModel.DiscordServer
{
    public class Role
    {
        public string Name { get; private set; }

        public Role(string name)
        {
            this.Name = name;
        }
    }
}
