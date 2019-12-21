using System;
using System.Collections.Generic;
using System.Linq;

namespace Watchman.Integrations.MongoDB
{
    public interface ISession : IDisposable
    {
        T Get<T>(Guid id) where T : Entity;
        IQueryable<T> Get<T>() where T : Entity;
        void Add<T>(T entity) where T : Entity;
        void Add<T>(IEnumerable<T> entities) where T : Entity;
        void AddOrUpdate<T>(T entity) where T : Entity;
        void Update<T>(T entity) where T : Entity;
        void Delete<T>(T entity) where T : Entity;
        void SaveChanges();
    }
}
