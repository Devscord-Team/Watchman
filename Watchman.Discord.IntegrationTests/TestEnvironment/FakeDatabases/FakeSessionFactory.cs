using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Integrations.Database;

namespace Watchman.Discord.IntegrationTests.TestEnvironment.FakeDatabases
{
    internal class FakeSessionFactory : ISessionFactory
    {
        public ISession CreateLite()
        {
            return new FakeSession();
        }

        public ISession CreateMongo()
        {
            return new FakeSession();
        }
    }

    internal class FakeSession : ISession
    {
        public Dictionary<string, List<object>> database = new Dictionary<string, List<object>>();

        public T Get<T>(Guid id) where T : Entity
        {
            return this.Get<T>().FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<T> Get<T>() where T : Entity
        {
            return this.GetCollection<T>();
        }

        public Task AddAsync<T>(T entity) where T : Entity
        {
            var name = this.GetCollectionName<T>();
            this.database[name].Add(entity);
            return Task.CompletedTask;
        }

        public async Task AddAsync<T>(IEnumerable<T> entities) where T : Entity
        {
            foreach (var entity in entities)
            {
                await this.AddAsync(entity);
            }
        }

        public Task AddOrUpdateAsync<T>(T entity) where T : Entity
        {
            if (this.Get<T>(entity.Id) == null)
            {
                return this.AddAsync(entity);
            }
            else
            {
                return this.UpdateAsync(entity);
            }
        }
        public Task UpdateAsync<T>(T entity) where T : Entity
        {
            this.DeleteAsync(entity);
            this.AddAsync(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync<T>(T entity) where T : Entity
        {
            var name = this.GetCollectionName<T>();
            var index = this.FindIndex(entity);
            if (index != -1)
            {
                this.database[name].RemoveAt(index);
            }
            return Task.CompletedTask;
        }

        private int FindIndex<T>(T entity) where T : Entity
        {
            var collection = this.GetCollection<T>();
            var found = collection
                .Select((x, i) => new { x, i })
                .FirstOrDefault(item => item.x.Id == entity.Id);
            return found?.i ?? -1;
        }

        private List<T> GetCollection<T>() where T : Entity
        {
            var name = this.GetCollectionName<T>();
            return this.database[name].Select(x => (T) x).ToList();
        }

        private string GetCollectionName<T>() where T : Entity
        {
            var typeName = typeof(T).FullName;
            if (!this.database.ContainsKey(typeName))
            {
                this.database.Add(typeName, new List<object>());
            }
            return typeName;
        }

        public void SaveChanges()
        {
        }
        public void Dispose()
        {
        }
    }
}
