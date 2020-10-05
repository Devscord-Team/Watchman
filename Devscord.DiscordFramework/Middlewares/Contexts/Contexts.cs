using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using System;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class Contexts
    {
        public DiscordServerContext Server { get; private set; }
        public ChannelContext Channel { get; private set; }
        public UserContext User { get; private set; }
        public MessageContext Message { get; private set; }

        public Contexts()
        {
        }

        public Contexts(DiscordServerContext server, ChannelContext channel, UserContext user)
        {
            this.Server = server;
            this.Channel = channel;
            this.User = user;
        }

        public Contexts(DiscordServerContext server, ChannelContext channel, UserContext user, MessageContext message)
        {
            this.Server = server;
            this.Channel = channel;
            this.User = user;
            this.Message = message;
        }

        public void SetContext<T>(T context) where T : IDiscordContext
        {
            switch (context)
            {
                case DiscordServerContext discordServerContext:
                    this.Server = discordServerContext;
                    break;

                case ChannelContext channelContext:
                    this.Channel = channelContext;
                    break;

                case UserContext userContext:
                    this.User = userContext;
                    break;

                case MessageContext messageContext:
                    this.Message = messageContext;
                    break;

                default:
                    throw new ArgumentException($"Context {context.GetType().Name} not exists in {typeof(Contexts).FullName}");
            }
        }
    }
}
