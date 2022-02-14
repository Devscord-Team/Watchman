using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.IoC;

namespace Watchman.Discord.IntegrationTests.TestEnvironment
{
    internal class TestWatchmanBotFactory
    {
        public BotCommandsRunner CreateCommandsRunner()
        {
            var configuration = new DiscordConfiguration();

            var builder = new ContainerBuilder();

            var container = new ContainerModule();
            builder = container.FillBuilder(builder);
            var context = builder.Build();

            var workflowBuilder = new WatchmanBot(configuration, context).GetWorkflowBuilder();
            workflowBuilder.Build();
            return new BotCommandsRunner();
        }
    }

    public class BotCommandsRunner
    {
        public async Task SendMessage(string text)
        {
            await Task.CompletedTask;
        }
    }
}
