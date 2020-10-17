namespace Watchman.Integrations.Database
{
    public interface ISessionFactory
    {
        ISession CreateMongo();
        ISession CreateLite();
    }
}
