using MongoDB.Driver;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Diagnostics;

namespace Watchman.Integrations.Logging
{
    public class SerilogInitializer
    {
        public static ILogger Initialize(IMongoDatabase mongoDatabase, Action<string> discordLog = null)
        {
#if DEBUG
            Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
#endif
            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .WriteTo.MongoDBCapped(mongoDatabase,
                    restrictedToMinimumLevel: LogEventLevel.Verbose,
                    collectionName: "Logs", cappedMaxSizeMb: 200)
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Verbose)
                .WriteTo.Debug(restrictedToMinimumLevel: LogEventLevel.Verbose)
                .WriteTo.Discord(discordLog)
                .CreateLogger();
            return logger;
        }
    }

    public class DiscordSink : ILogEventSink
    {
        private readonly Action<string> _onEmit;

        public DiscordSink(Action<string> onEmit = null)
        {
            _onEmit = onEmit;
        }

        public void Emit(LogEvent logEvent)
        {
            if(_onEmit == null)
            {
                return;
            }

            var message = logEvent.RenderMessage();
            _onEmit.Invoke(message);
        }
    }

    public static class SinkExtensions
    {
        public static LoggerConfiguration Discord(this LoggerSinkConfiguration loggerConfiguration, Action<string> onEmit)
        {
            return loggerConfiguration.Sink(new DiscordSink(onEmit));
        }
    }
}
