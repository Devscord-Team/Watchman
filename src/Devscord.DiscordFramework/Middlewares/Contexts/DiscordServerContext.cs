using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
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

        public DiscordServerContext(ulong id, string name, UserContext owner, ChannelContext landingChannel, IEnumerable<ChannelContext> textChannels)
        {
            this.Id = id;
            this.Name = name;
            this.Owner = owner;
            this.LandingChannel = landingChannel;
            this.TextChannels = textChannels;
        }
    }
}
