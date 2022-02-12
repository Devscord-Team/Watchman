using Devscord.DiscordFramework.Architecture.Middlewares;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    public interface IContextFactory<T, U> where U : IDiscordContext
    {
        U Create(T socketObject);
    }
}
