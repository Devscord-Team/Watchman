using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    interface IContextFactory<T, U> where U : IDiscordContext
    {
        U Create(T socketObject);
    }
}
