using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using System;
using System.Collections.Generic;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class DiscordServerContext : IDiscordContext
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }
        public UserContext Owner { get; private set; }
        public ChannelContext LandingChannel { get; private set; }
        public IEnumerable<ChannelContext> TextChannels { get; private set; }
        public IAsyncEnumerable<UserContext> Users => this._users ??= this._getServerUsers.Invoke(this);

        private readonly Func<DiscordServerContext, IAsyncEnumerable<UserContext>> _getServerUsers;
        private IAsyncEnumerable<UserContext> _users;

        public DiscordServerContext(ulong id, string name, UserContext owner, ChannelContext landingChannel, IEnumerable<ChannelContext> textChannels, Func<DiscordServerContext, IAsyncEnumerable<UserContext>> getServerUsers)
        {
            this._getServerUsers = getServerUsers;
            this.Id = id;
            this.Name = name;
            this.Owner = owner;
            this.LandingChannel = landingChannel;
            this.TextChannels = textChannels;
        }
    }
}
