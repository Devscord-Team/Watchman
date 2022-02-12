using Devscord.DiscordFramework.Architecture.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class DiscordServerContext : IDiscordContext
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }
        public ChannelContext LandingChannel { get; private set; }

        private readonly Func<DiscordServerContext, IEnumerable<UserContext>> _getServerUsers;
        private readonly Func<DiscordServerContext, IEnumerable<UserRole>> _getServerRoles;
        private readonly Func<DiscordServerContext, IEnumerable<ChannelContext>> _getTextChannels;
        private readonly Func<UserContext> _getOwner;

        public DiscordServerContext(ulong id, string name, Func<UserContext> owner, ChannelContext landingChannel, Func<DiscordServerContext, IEnumerable<ChannelContext>> getTextChannels, Func<DiscordServerContext, IEnumerable<UserContext>> getServerUsers, Func<DiscordServerContext, IEnumerable<UserRole>> getServerRoles)
        {
            this._getServerUsers = getServerUsers;
            this._getServerRoles = getServerRoles;
            this.Id = id;
            this.Name = name;
            this._getOwner = owner;
            this._getTextChannels = getTextChannels;
            this.LandingChannel = landingChannel;
        }

        public IEnumerable<ChannelContext> GetTextChannels() => this._getTextChannels.Invoke(this);
        public ChannelContext GetTextChannel(ulong channelId) => this.GetTextChannels().FirstOrDefault(x => x.Id == channelId);
        public IEnumerable<UserContext> GetUsers() => this._getServerUsers.Invoke(this); // todo: use static list and events
        public IEnumerable<UserRole> GetRoles() => this._getServerRoles.Invoke(this);
        public UserContext GetOwner() => this._getOwner.Invoke();
    }
}
