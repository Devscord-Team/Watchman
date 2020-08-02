namespace Watchman.Cqrs
{
    public interface IQuery
    {

    }

    public interface IQuery<T> where T : IQueryResult
    {

    }
}