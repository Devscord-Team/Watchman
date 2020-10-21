using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Integrations.Database;

namespace Watchman.Integrations.Database.MongoDB
{
    public class MongoSession : ISession
    {
        private readonly IMongoDatabase _database;

        public MongoSession(IMongoDatabase database)
        {
            this._database = database;
        }

        public T Get<T>(Guid id) where T : Entity
        {
            return this.Get<T>().FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<T> Get<T>() where T : Entity
        {
            return this.GetCollection<T>().AsQueryable();
        }

        public async Task AddAsync<T>(T entity) where T : Entity
        {
            await this.GetCollection<T>().InsertOneAsync(entity);
        }

        public async Task AddAsync<T>(IEnumerable<T> entities) where T : Entity
        {
            await this.GetCollection<T>().InsertManyAsync(entities);
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

        public async Task UpdateAsync<T>(T entity) where T : Entity
        {
            if (entity.IsChanged())
            {
                await this.GetCollection<T>().ReplaceOneAsync(x => x.Id == entity.Id, entity);
            }
        }

        public async Task DeleteAsync<T>(T entity) where T : Entity
        {
            await this.GetCollection<T>().DeleteOneAsync(x => x.Id == entity.Id);
        }

        public void SaveChanges()
        {
            //use on database change
        }

        public void Dispose()
        {
            //use on database change
        }

        private IMongoCollection<T> GetCollection<T>() where T : Entity
        {
            return this._database.GetCollection<T>($"{typeof(T).Name}s");
        }
    }
}
