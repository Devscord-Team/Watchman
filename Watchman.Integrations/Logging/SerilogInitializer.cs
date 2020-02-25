using MongoDB.Driver;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.MongoDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Watchman.Integrations.Logging
{
    public class SerilogInitializer
    {
        private static bool initialized;

        public static void Initialize(IMongoDatabase mongoDatabase)
        {
            if(initialized)
            {
                return;
            }

            var formatter = new CompactJsonFormatter();
            Log.Logger = new LoggerConfiguration()
                .WriteTo.RollingFile(formatter, "logs/log-{Date}.txt", buffered: true)
                .WriteTo.MongoDBCapped(mongoDatabase,
                    restrictedToMinimumLevel: LogEventLevel.Debug,
                    period: TimeSpan.Zero,
                    collectionName: "logs",
                    cappedMaxSizeMb: 100,
                    cappedMaxDocuments: 5000,
                    mongoDBJsonFormatter: new MongoDBJsonFormatter(omitEnclosingObject: false, renderMessage: true))
                .WriteTo.Console(formatter)
                .CreateLogger();

            initialized = true;
        }
    }
}
