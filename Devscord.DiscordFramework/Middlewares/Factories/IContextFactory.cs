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

    interface IContextFactory<T1, T2, U> where U : IDiscordContext
    {
        U Create(T1 socketObject, T2 socketObject2);
    }
}
