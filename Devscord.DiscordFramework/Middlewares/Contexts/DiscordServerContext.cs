using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Newtonsoft.Json;
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
        [JsonIgnore] public IAsyncEnumerable<UserContext> Users => this._getServerUsers.Invoke(this); // todo: use static list and events
        [JsonIgnore] public IEnumerable<UserRole> Roles => this._getServerRoles.Invoke(this);

        private readonly Func<DiscordServerContext, IAsyncEnumerable<UserContext>> _getServerUsers;
        private readonly Func<DiscordServerContext, IEnumerable<UserRole>> _getServerRoles;

        public DiscordServerContext(ulong id, string name, UserContext owner, ChannelContext landingChannel, IEnumerable<ChannelContext> textChannels, Func<DiscordServerContext, IAsyncEnumerable<UserContext>> getServerUsers, Func<DiscordServerContext, IEnumerable<UserRole>> getServerRoles)
        {
            this._getServerUsers = getServerUsers;
            this._getServerRoles = getServerRoles;
            this.Id = id;
            this.Name = name;
            this.Owner = owner;
            this.LandingChannel = landingChannel;
            this.TextChannels = textChannels;
        }
    }
}
