using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Watchman.Integrations.MongoDB
{
    public class Session : ISession
    {
        private readonly IMongoDatabase _database;

        public Session(IMongoDatabase database)
        {
            this._database = database;
        }

        public T Get<T>(Guid id) where T : Entity
        {
            return this.Get<T>().FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<T> Get<T>() where T : Entity
        {
            return this.GetCollection<T>().AsQueryable().Where(x => !x.IsDeleted);
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
            await this.GetCollection<T>().ReplaceOneAsync(x => x.Id == entity.Id, entity);
        }

        public async Task DeleteAsync<T>(T entity) where T : Entity
        {
            await this.GetCollection<T>().UpdateOneAsync(x => x.Id == entity.Id, Builders<T>.Update.Set(p => p.IsDeleted, true));
        }

        public async Task DeleteAsync<T>(Expression<Func<T, bool>> filter) where T : Entity
        {
            await this.GetCollection<T>().UpdateManyAsync(filter, Builders<T>.Update.Set(p => p.IsDeleted, true));
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
