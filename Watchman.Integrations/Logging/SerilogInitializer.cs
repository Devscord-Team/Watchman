using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Serilog.Sinks.Seq;

namespace Watchman.Integrations.Logging
{
    [ExcludeFromCodeCoverage]
    public class SerilogInitializer
    {
        public static ILogger Initialize(IConfigurationRoot configuration)
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
                .WriteTo.Console(
                    restrictedToMinimumLevel: LogEventLevel.Warning,
                    outputTemplate: "[{Timestamp:dd-MM-yyyy} - {Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.Debug(restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.Seq(configuration.GetConnectionString("Seq"), LogEventLevel.Information)
                .CreateLogger();
            return logger;
        }
    }
}
