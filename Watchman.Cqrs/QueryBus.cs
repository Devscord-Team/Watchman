using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;

namespace Watchman.Cqrs
{
    public class QueryBus : IQueryBus
    {
        private readonly IComponentContext _context;
        public QueryBus(IComponentContext context)
        {
            _context = context;
        }

        public W Execute<W>(IQuery<W> query) where W : IQueryResult
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query),
                    $"Query: '{typeof(W)}' can not be null.");
            }
            var handlerType = typeof(IQueryHandler<,>)
                .MakeGenericType(query.GetType(), typeof(W));
            dynamic handler = _context.Resolve(handlerType);
            return handler.Handle((dynamic)query);
        }

        public Task<W> ExecuteAsync<W>(IQuery<W> query) where W : IQueryResult
        {
            //TODO make it better
            return Task.FromResult(this.Execute(query));
        }
    }
}