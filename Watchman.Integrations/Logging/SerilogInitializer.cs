using MongoDB.Driver;
using Newtonsoft.Json;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Compact;
using Serilog.Sinks.MongoDB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Watchman.Integrations.Logging
{
    public class SerilogInitializer
    {
        public static ILogger Initialize(IMongoDatabase mongoDatabase, Action<string> discordLog = null)
        {
#if DEBUG
            Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
#endif

            var formatter = new CompactJsonFormatter(new Serilog.Formatting.Json.JsonValueFormatter());
            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .WriteTo.RollingFile(formatter, "logs/log-{Date}.json", buffered: true)
                .WriteTo.MongoDB(mongoDatabase,
                    restrictedToMinimumLevel: LogEventLevel.Verbose,
                    collectionName: "Logs"
                    //mongoDBJsonFormatter: new MongoDBJsonFormatter())
                    )
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Verbose)
                .WriteTo.Debug(restrictedToMinimumLevel: LogEventLevel.Verbose)
                .WriteTo.Discord(formatter, discordLog)
                .CreateLogger();
            return logger;
        }
    }

    public class DiscordSink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;
        private readonly ITextFormatter _textFormatter;
        private readonly Action<string> _onEmit;

        public DiscordSink(IFormatProvider formatProvider, ITextFormatter textFormatter, Action<string> onEmit = null)
        {
            _formatProvider = formatProvider;
            _textFormatter = textFormatter;
            _onEmit = onEmit;
        }

        public void Emit(LogEvent logEvent)
        {
            if(_onEmit == null)
            {
                return;
            }
            var writer = new StringWriter(this._formatProvider);
            _textFormatter.Format(logEvent, writer);
            var deserialized = JsonConvert.DeserializeObject(writer.ToString());
            var message = JsonConvert.SerializeObject(deserialized, Formatting.Indented) + "\r\n\r\n" + logEvent.RenderMessage();
            _onEmit.Invoke(message);
        }
    }

    public static class SinkExtensions
    {
        public static LoggerConfiguration Discord(this LoggerSinkConfiguration loggerConfiguration, ITextFormatter textFormatter, Action<string> onEmit, IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.Sink(new DiscordSink(formatProvider, textFormatter, onEmit));
        }
    }
}
