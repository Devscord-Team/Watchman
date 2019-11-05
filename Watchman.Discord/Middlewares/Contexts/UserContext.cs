using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Discord.Framework.Architecture.Middlewares;

namespace Watchman.Discord.Middlewares.Contexts
{
    public class UserContext : IDiscordContext
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }

        public UserContext(ulong id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
