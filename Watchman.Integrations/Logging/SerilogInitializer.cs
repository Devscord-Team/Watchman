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
        public static ILogger Initialize(IMongoDatabase mongoDatabase)
        {
            var formatter = new CompactJsonFormatter();
            var logger = new LoggerConfiguration()
                .WriteTo.RollingFile(formatter, "logs/log-{Date}.txt", buffered: true)
                .WriteTo.MongoDBCapped(mongoDatabase,
                    restrictedToMinimumLevel: LogEventLevel.Debug,
                    period: TimeSpan.Zero,
                    collectionName: "logs",
                    cappedMaxSizeMb: 100,
                    cappedMaxDocuments: 5000,
                    mongoDBJsonFormatter: new MongoDBJsonFormatter(omitEnclosingObject: false, renderMessage: true))
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Verbose)
                .WriteTo.Debug(restrictedToMinimumLevel: LogEventLevel.Verbose)
                .CreateLogger();
            return logger;
        }
    }
}
