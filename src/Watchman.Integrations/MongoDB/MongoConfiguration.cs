﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using System.Collections.Generic;

namespace Watchman.Integrations.MongoDB
{
    public static class MongoConfiguration
    {
        private static bool _initialized;

        public static void Initialize()
        {
            if (_initialized)
            {
                return;
            }
            RegisterConventions();
            _initialized = true;
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
