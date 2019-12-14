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
            var contextName = context.GetType().Name;
            switch (contextName)
            {
                case "DiscordServerContext":
                    Server = context as DiscordServerContext;
                    break;

                case "ChannelContext":
                    Channel = context as ChannelContext;
                    break;

                case "UserContext":
                    User = context as UserContext;
                    break;
            }
        }
    }
}
