﻿using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDBMigrations;
using Serilog;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Watchman.Integrations.Database.MongoDB
{
    [ExcludeFromCodeCoverage]
    public static class MongoConfiguration
    {
        private static bool _initialized;

        public static void Initialize(IConfigurationRoot configuration)
        {
            if (_initialized)
            {
                return;
            }
            RegisterConventions();
            ApplyMigrations(configuration);
            _initialized = true;
        }

        private static void RegisterConventions()
        {
            ConventionRegistry.Register("DevscordConventions", new MongoConventions(), x => true);
        }

        private static void ApplyMigrations(IConfigurationRoot configuration)
        {
            var connectionString = configuration.GetConnectionString("Mongo");
            var databaseName = connectionString.Split('/').Last();
            var assemblyForMigrations = Assembly.GetExecutingAssembly();
            
            var result = new MigrationEngine()
                .UseDatabase(connectionString, databaseName)
                .UseAssembly(assemblyForMigrations)
                .UseSchemeValidation(enabled: false)
                .UseProgressHandler(context => 
                    Log.Information($@"The migration with name ""{context.MigrationName}"" from {context.TargetVersion} version was processed as {context.CurrentNumber} from all {context.TotalCount} migrations"))
                .Run();

            if (!result.Success)
            {
                // TODO: Do something when migrations throw exceptions.
            }

            Log.Information($"The current version of the database after data migrations is {result.CurrentVersion}");
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
