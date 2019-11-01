namespace Watchman.Integrations.MongoDB
{
    public interface ISessionFactory
    {
        ISession Create();
    }
}
