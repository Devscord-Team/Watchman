using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;

namespace Watchman.Integrations.MongoDB
{
    public static class MongoConfiguration
    {
        private static bool _initialized;
        private static object Lock = new object();

        public static void Initialize()
        {
            lock (Lock)
            {
                if (_initialized)
                {
                    return;
                }
                RegisterConventions();
                _initialized = true;
            }
        }

        private static void RegisterConventions()
        {
            ConventionRegistry.Register("DevscordConventions", new MongoConventions(), x => true);
        }

        private class MongoConventions : IConventionPack
        {
            public IEnumerable<IConvention> Conventions => new List<IConvention>
            {
                new IgnoreExtraElementsConvention(true),
                new EnumRepresentationConvention(BsonType.String),
                new CamelCaseElementNameConvention()
            };
        }
    }
}
