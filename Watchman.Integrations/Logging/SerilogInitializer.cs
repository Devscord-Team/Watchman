using MongoDB.Driver;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using System;
using System.Diagnostics;

namespace Watchman.Integrations.Logging
{
    public class SerilogInitializer
    {
        public static ILogger Initialize(IMongoDatabase mongoDatabase)
        {
#if DEBUG
            Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
#endif
            var logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .WriteTo.File(new JsonFormatter(), "logs/all/log-{Date}.json", rollingInterval: RollingInterval.Day, shared: true, flushToDiskInterval: TimeSpan.FromSeconds(15), restrictedToMinimumLevel: LogEventLevel.Verbose)
                .WriteTo.File(new JsonFormatter(closingDelimiter: ","), "logs/warningplus/log-{Date}.json", rollingInterval: RollingInterval.Day, shared: true, flushToDiskInterval: TimeSpan.FromSeconds(15), restrictedToMinimumLevel: LogEventLevel.Warning)
                .WriteTo.File(new JsonFormatter(closingDelimiter: ","), "logs/errorplus/log-{Date}.json", rollingInterval: RollingInterval.Day, shared: true, flushToDiskInterval: TimeSpan.FromSeconds(15), restrictedToMinimumLevel: LogEventLevel.Error)
                .WriteTo.Console(
                    restrictedToMinimumLevel: LogEventLevel.Verbose,
                    outputTemplate: "[{Timestamp:dd-MM-yyyy} - {Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.Debug(restrictedToMinimumLevel: LogEventLevel.Verbose)
                .CreateLogger();
            return logger;
        }
    }
}
