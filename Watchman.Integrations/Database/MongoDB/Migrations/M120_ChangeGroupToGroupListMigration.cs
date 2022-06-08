using MongoDB.Driver;
using MongoDBMigrations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Watchman.Integrations.Database.MongoDB.Migrations
{
    internal class M120_ChangeGroupToGroupListMigration : IMigration
    {
        public MongoDBMigrations.Version Version => new(1, 2, 0);

        public string Name => "Change type of group field from ConfigurationItems to groups array (string => Array<string>).";

        public void Up(IMongoDatabase database)
        {
            var collection = database.GetCollection<ItemOldVersion>("ConfigurationItems");
            var newCollection = database.GetCollection<ConfigurationItemNewVersion>("ConfigurationItems");

            foreach (var org in collection.AsQueryable())
            {
                var newItem = new ConfigurationItemNewVersion(org.Id, org.CreatedAt, org.UpdatedAt, org.Version, org.Value, org.ServerId, org.Name, 
                    groups: new List<string> { org.Group }, org.SubGroup, org.NameCopy);

                collection.DeleteOne(x => x.Id == org.Id);
                newCollection.InsertOne(newItem);
            }
        }

        public void Down(IMongoDatabase database)
        {
            var previousCollection = database.GetCollection<ItemOldVersion>("ConfigurationItems");
            var currentCollection = database.GetCollection<ConfigurationItemNewVersion>("ConfigurationItems");

            foreach (var currentVersion in currentCollection.AsQueryable())
            {
                var previousVersion = new ItemOldVersion(currentVersion.Id, currentVersion.CreatedAt, currentVersion.UpdatedAt, currentVersion.Version, currentVersion.Value, currentVersion.ServerId, currentVersion.Name, 
                    group: currentVersion.Groups.First(), currentVersion.SubGroup, currentVersion.NameCopy);

                currentCollection.DeleteOne(x => x.Id == currentVersion.Id);
                previousCollection.InsertOne(previousVersion);
            }
        }

        private class ConfigurationItemNewVersion : Entity
        {
            public object Value { get; set; }
            public ulong ServerId { get; set; }
            public string Name { get; set; }
            public List<string> Groups { get; set; }
            public string SubGroup { get; set; }
            public string NameCopy { get; set; }

            public ConfigurationItemNewVersion(Guid id, DateTime createdAt, DateTime updatedAt, int version, object value, ulong serverId, string name, List<string> groups, string subGroup, string nameCopy)
            {
                this.Id = id;
                this.CreatedAt = createdAt;
                this.UpdatedAt = updatedAt;
                this.Version = version;
                this.Value = value;
                this.ServerId = serverId;
                this.Name = name;
                this.Groups = groups;
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
            public string NameCopy { get; set; }

            public ItemOldVersion(Guid id, DateTime createdAt, DateTime updatedAt, int version, object value, ulong serverId, string name, string group, string subGroup, string nameCopy)
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
    }
}
