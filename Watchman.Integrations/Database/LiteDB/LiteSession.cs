using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watchman.Integrations.Database.LiteDB
{
    public class LiteSession : ISession
    {
        private readonly ILiteDatabase liteDatabase;

        public LiteSession(ILiteDatabase liteDatabase)
        {
            this.liteDatabase = liteDatabase;
        }

        public Task AddAsync<T>(T entity) where T : Entity
        {
            throw new NotImplementedException();
        }

        public Task AddAsync<T>(IEnumerable<T> entities) where T : Entity
        {
            throw new NotImplementedException();
        }

        public Task AddOrUpdateAsync<T>(T entity) where T : Entity
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync<T>(T entity) where T : Entity
        {
            throw new NotImplementedException();
        }

        public T Get<T>(Guid id) where T : Entity
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Get<T>() where T : Entity
        {
            throw new NotImplementedException();
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync<T>(T entity) where T : Entity
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            this.liteDatabase.Dispose();
        }
    }
}
