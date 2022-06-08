using MongoDB.Driver;
using MongoDBMigrations;
using System;
using System.Linq;

namespace Watchman.Integrations.Database.MongoDB.Migrations
{
    internal class M110_AddNameCopyInConfigurationItemMigration : IMigration // All migrations files must have a class with IMigration interface.
    {
        public MongoDBMigrations.Version Version => new(1, 1, 0); // Version database when the migration will be applied (the used pattern of versions is named Semantic Versioning).

        public string Name => "Add NameCopy field to ConfigurationItem as a copy of Name field."; // The migration's name (description).

        public void Up(IMongoDatabase database) // This is used when code has to upgrade database with this migration.
        {
            var collection = database.GetCollection<ConfigurationItemNewVersion>("ConfigurationItems");

            foreach (var original in collection.AsQueryable())
            {
                var newItem = new ConfigurationItemNewVersion(original.Id, original.CreatedAt, original.UpdatedAt, original.Version,
                    original.Value, original.ServerId, original.Name, original.Group, original.SubGroup,
                    nameCopy: original.Name);

                collection.DeleteOne(x => x.Id == original.Id);
                collection.InsertOne(newItem);
            }
        }

        public void Down(IMongoDatabase database) // It's for downgrade database.
        {
            var collection = database.GetCollection<ItemOldVersion>("ConfigurationItems");

            foreach (var originalItem in collection.AsQueryable())
            {
                collection.DeleteOne(x => x.Id == originalItem.Id);
                collection.InsertOne(originalItem);
            }
        }

        private class ConfigurationItemNewVersion : Entity
        {
            public object Value { get; set; }
            public ulong ServerId { get; set; }
            public string Name { get; set; }
            public string Group { get; set; }
            public string SubGroup { get; set; }
            public string NameCopy { get; set; }

            public ConfigurationItemNewVersion(Guid id, DateTime createdAt, DateTime updatedAt, int version, object value, ulong serverId, string name, string group, string subGroup, string nameCopy)
            {
                this.Id = id;
                this.CreatedAt = createdAt;
                this.UpdatedAt = updatedAt;
                this.Version = version;
                this.Value = value;
                this.ServerId = serverId;
                this.Name = name;
                this.Group = group;
                this.SubGroup = subGroup;
                this.NameCopy = nameCopy;
            }
        }

        private class ItemOldVersion : Entity
        {
            public object Value { get; set; }
            public ulong ServerId { get; set; }
            public string Name { get; set; }
            public string Group { get; set; }
            public string SubGroup { get; set; }
        }
    } 
}
