using LiteDB;

using MongoDB.Bson;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watchman.Integrations.Database.LiteDB
{
    public class LiteSession : ISession
    {
        private readonly ILiteDatabase _database;

        public LiteSession(ILiteDatabase liteDatabase)
        {
            this._database = liteDatabase;
        }

        public T Get<T>(Guid id) where T : Entity
        {
            return this.GetCollection<T>().FindById(id);
        }

        public IEnumerable<T> Get<T>() where T : Entity
        {
            return this.GetCollection<T>().Query().ToEnumerable();
        }

        public Task AddAsync<T>(T entity) where T : Entity
        {
            this.GetCollection<T>().Insert(entity.Id, entity);
            return Task.CompletedTask;
        }

        public Task AddAsync<T>(IEnumerable<T> entities) where T : Entity
        {
            this.GetCollection<T>().InsertBulk(entities);
            return Task.CompletedTask;
        }

        public async Task AddOrUpdateAsync<T>(T entity) where T : Entity
        {
            if (this.Get<T>(entity.Id) == null)
            {
                await this.AddAsync(entity);
            }
            else
            {
                await this.UpdateAsync(entity);
            }
        }

        public Task UpdateAsync<T>(T entity) where T : Entity
        {
            this.GetCollection<T>().Update(entity.Id, entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync<T>(T entity) where T : Entity
        {
            this.GetCollection<T>().Delete(entity.Id);
            return Task.CompletedTask;
        }

        public void SaveChanges()
        {
            //use on database change
        }

        public void Dispose()
        {
            this._database.Dispose();
        }

        private ILiteCollection<T> GetCollection<T>() where T : Entity
        {
            return this._database.GetCollection<T>($"{typeof(T).Name}s", BsonAutoId.Guid);
        }
    }
}
