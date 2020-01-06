using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class Contexts
    {
        public DiscordServerContext Server { get; private set; }
        public ChannelContext Channel { get; private set; }
        public UserContext User { get; private set; }

        public void SetContext<T>(T context) where T : IDiscordContext
        {
            switch (context)
            {
                case DiscordServerContext discordServerContext:
                    Server = discordServerContext;
                    break;

                case ChannelContext channelContext:
                    Channel = channelContext;
                    break;

                case UserContext userContext:
                    User = userContext;
                    break;

                default:
                    throw new ArgumentException($"Context {context.GetType().Name} not exists in {typeof(Contexts).FullName}");
            }
        }
    }
}
