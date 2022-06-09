using MongoDB.Driver;
using MongoDBMigrations;
using System;
using System.Linq;

namespace Watchman.Integrations.Database.MongoDB.Migrations
{
    internal class M120_CreateBackupOfInitEventsMigration : IMigration
    {
        public MongoDBMigrations.Version Version => new(1, 2, 0);

        public string Name => "Create backup of InitEvents collection";

        public void Up(IMongoDatabase database)
        {
            if (!database.ListCollectionNames().ToList().Any(x => x == "InitEvents"))
            {
                return;
            }

            var events = database.GetCollection<InitEvent>("InitEvents").AsQueryable();

            database.CreateCollection("InitEvents_backup");
            database.GetCollection<InitEvent>("InitEvents_backup").InsertMany(events);
        }

        public void Down(IMongoDatabase database)
        {
            if (!database.ListCollectionNames().ToList().Any(x => x == "InitEvents_backup"))
            {
                return;
            }
            database.DropCollection("InitEvents_backup");
        }

        private class InitEvent : Entity
        {
            public ulong ServerId { get; set; }
            public DateTime EndedAt { get; set; }
        }
    }
}
