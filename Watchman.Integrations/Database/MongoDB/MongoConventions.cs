using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using System.Collections.Generic;

namespace Watchman.Integrations.Database.MongoDB
{
    internal class MongoConventions : IConventionPack
    {
        public IEnumerable<IConvention> Conventions => new List<IConvention>
        {
            new IgnoreExtraElementsConvention(true),
            new EnumRepresentationConvention(BsonType.String),
            new CamelCaseElementNameConvention()
        };
    }
}
