using MongoDB.Driver;
using MongoDBMigrations;
using System;
using System.Linq;

namespace Watchman.Integrations.Database.MongoDB.Migrations
{
    internal class M130_AddEndedDayHourMinuteToInitEventsMigration : IMigration
    {
        public MongoDBMigrations.Version Version => new(1, 3, 0);

        public string Name => "Add fields EndedDay, EndedHour, EndedMinute to InitEvents collection.";

        public void Up(IMongoDatabase database)
        {
            var collection = database.GetCollection<InitEvent>("InitEvents");

            foreach (var initEvent in collection.AsQueryable())
            {
                // TODO: use Automapper?
                initEvent.EndedDay = initEvent.EndedAt.Day;
                initEvent.EndedHour = initEvent.EndedAt.Hour;
                initEvent.EndedMinute = initEvent.EndedAt.Minute;

                collection.ReplaceOne(x => x.Id == initEvent.Id, initEvent);
            }
        }

        public void Down(IMongoDatabase database)
        {
            var oldCollection = database.GetCollection<InitEvent>("InitEvents");
            var newCollection = database.GetCollection<EventOldVersion>("InitEvents");

            foreach (var org in oldCollection.AsQueryable())
            {
                var initEvent = new EventOldVersion(org.Id, org.CreatedAt, org.UpdatedAt, org.Version, org.ServerId, org.EndedAt);

                oldCollection.DeleteOne(x => x.Id == org.Id);
                newCollection.InsertOne(initEvent);
            }
        }

        private class InitEvent : Entity
        {
            public ulong ServerId { get; set; }
            public DateTime EndedAt { get; set; }
            public int EndedDay { get; set; }
            public int EndedHour { get; set; }
            public int EndedMinute { get; set; }

            public InitEvent(Guid id, DateTime createdAt, DateTime updatedAt, int version, ulong serverId, DateTime endedAt, int endedDay, int endedHour, int endedMinute)
            {
                this.Id = id;
                this.CreatedAt = createdAt;
                this.UpdatedAt = updatedAt;
                this.Version = version;
                this.ServerId = serverId;
                this.EndedAt = endedAt;
                this.EndedDay = endedDay;
                this.EndedHour = endedHour;
                this.EndedMinute = endedMinute;
            }
        }

        private class EventOldVersion : Entity
        {
            public ulong ServerId { get; set; }
            public DateTime EndedAt { get; set; }

            public EventOldVersion(Guid id, DateTime createdAt, DateTime updatedAt, int version, ulong serverId, DateTime endedAt)
            {
                this.Id = id;
                this.CreatedAt = createdAt;
                this.UpdatedAt = updatedAt;
                this.Version = version;
                this.ServerId = serverId;
                this.EndedAt = endedAt;
            }
        }
    }
}
