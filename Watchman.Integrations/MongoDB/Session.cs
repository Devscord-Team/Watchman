using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Watchman.Integrations.MongoDB
{
    public class Session : ISession
    {
        private readonly IMongoDatabase _database;

        public Session(IMongoDatabase database)
        {
            _database = database;
        }

        public T Get<T>(Guid id) where T : Entity
        {
            return this.Get<T>().FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<T> Get<T>() where T : Entity
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
            await this.GetCollection<T>().ReplaceOneAsync(x => x.Id == entity.Id, entity);
        }

        public async Task DeleteAsync<T>(T entity) where T : Entity
        {
            await this.GetCollection<T>().DeleteOneAsync(x => x.Id == entity.Id);
        }

        public async Task ReplaceByTypeAsync<T>(T item) where T : Entity
        {
            var possiblePreviousItems = this.Get<T>().ToList();
            if (possiblePreviousItems.Count > 1)
            {
                throw new Exception("To replace there must be only one item of this type");
            }
            if (possiblePreviousItems.Count == 1)
            {
                await this.DeleteAsync(possiblePreviousItems.First());
            }
            await this.AddAsync(item);
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
            return _database.GetCollection<T>($"{typeof(T).Name}s");
        }
    }
}
