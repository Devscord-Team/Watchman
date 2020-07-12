using System;
using Newtonsoft.Json;
using Serilog;
using System.IO;
using System.Threading.Tasks;
using Devscord.DiscordFramework;

namespace Watchman.Discord
{
    internal class Program
    {
        private const int HOW_MANY_EXCEPTIONS_IN_SHORT_TIME_TO_STOP_BOT = 4;
        private static int _numberOfExceptionsInLastTime;
        private static DateTime _lastException = DateTime.MinValue;

        public static async Task Main(string[] args)
        {
#if DEBUG
            var configPath = "config.json";
#elif RELEASE
            var configPath = "config-prod.json";
#endif
            var configuration = JsonConvert.DeserializeObject<DiscordConfiguration>(File.ReadAllText(configPath));

            var watchman = new WatchmanBot(configuration);
            var workflowBuilder = watchman.GetWorkflowBuilder();

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                HandleException((Exception) e.ExceptionObject);
            };
            while (true)
            {
                TryToRun(workflowBuilder).Wait();
            }
        }

        private static async Task TryToRun(WorkflowBuilder workflowBuilder)
        {
            if (_numberOfExceptionsInLastTime++ >= HOW_MANY_EXCEPTIONS_IN_SHORT_TIME_TO_STOP_BOT)
            {
                throw new Exception("Too many restarts");
            }
            try
            {
                workflowBuilder.Build();
                await Task.Delay(-1);
            }
            catch (Exception e)
            {
                HandleException(e);
                await Task.Delay(10000);
            }
        }

        private static void HandleException(Exception e)
        {
            Log.Error(e, "Not handled exception!");

            if (_lastException < DateTime.Now.AddMinutes(-30))
            {
                _numberOfExceptionsInLastTime = 0;
            }
            _lastException = DateTime.Now;
        }
    }
}
