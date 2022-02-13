using Autofac;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Watchman.Cqrs
{
    public class QueryBus : IQueryBus
    {
        private readonly IComponentContext _context;
        public QueryBus(IComponentContext context)
        {
            this._context = context;
        }

        public W Execute<W>(IQuery<W> query) where W : IQueryResult
        {
            //Log.Debug("Query: {query}", query);
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query),
                    $"Query: '{typeof(W)}' can not be null.");
            }
            var handlerType = typeof(IQueryHandler<,>)
                .MakeGenericType(query.GetType(), typeof(W));
            dynamic handler = this._context.Resolve(handlerType);
            return handler.Handle((dynamic) query);
        }

        public Task<W> ExecuteAsync<W>(IQuery<W> query) where W : IQueryResult
        {
            return Task.FromResult(this.Execute(query));
        }
    }
}