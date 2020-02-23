using System.Threading.Tasks;

namespace Watchman.Cqrs
{
    public interface IQueryBus
    {
        W Execute<W>(IQuery<W> query) where W : IQueryResult;
        Task<W> ExecuteAsync<W>(IQuery<W> query) where W : IQueryResult;
    }
    
}