using AutoFixture;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watchman.Discord.UnitTests.TestObjectFactories
{
    internal class TestContextsFactory
    {
        private readonly Fixture fixture = new ();

        public Contexts CreateContexts(ulong serverId, ulong channelId, ulong userId, string avatarUrl = null)
        {
            var server = this.CreateServerContext(serverId);
            var channel = this.CreateChannelContext(channelId);
            var user = this.CreateUserContext(userId, avatarUrl);
            var message = this.CreateMessageContext();
            return new Contexts(server, channel, user, message);
        }

        public DiscordServerContext CreateServerContext(ulong serverId)
        {
            //todo possiblity to provide configuration
            return new DiscordServerContext(
                serverId, 
                fixture.Create<string>(), 
                () => this.CreateUserContext(fixture.Create<ulong>()),
                this.CreateChannelContext(fixture.Create<ulong>()),
                x => new List<ChannelContext>() { this.CreateChannelContext(fixture.Create<ulong>()) },
                x => new List<UserContext>() { this.CreateUserContext(fixture.Create<ulong>()) },
                x => new List<UserRole>() { new UserRole(fixture.Create<ulong>(), fixture.Create<string>(), x.Id) }
                );
        }

        public ChannelContext CreateChannelContext(ulong channelId)
        {
            return new ChannelContext(channelId, fixture.Create<string>());
        }

        public UserContext CreateUserContext(ulong userId, string avatarUrl = null)
        {
            return new UserContext(
                userId,
                fixture.Create<string>(),
                new List<UserRole>() { new UserRole(fixture.Create<ulong>(), fixture.Create<string>(), fixture.Create<ulong>()) },
                avatarUrl,
                fixture.Create<string>(),
                x => false,
                x => DateTime.UtcNow.AddDays(-10)
                );
        }

        public MessageContext CreateMessageContext()
        {
            return new MessageContext(fixture.Create<ulong>(), DateTime.UtcNow.AddMilliseconds(-500), false);
        }
    }
}
