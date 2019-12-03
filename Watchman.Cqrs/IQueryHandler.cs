using System.Threading.Tasks;

namespace Watchman.Cqrs
{
    public interface IQueryHandler
    {

    }

    public interface IQueryHandler<in T, out W> : IQueryHandler
        where T : IQuery<W>
        where W : IQueryResult

    {
        W Handle(T query);
    }
    
}