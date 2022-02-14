using Autofac;
using Devscord.DiscordFramework.Integration.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Discord.IntegrationTests.TestEnvironment.FakeClients;
using Watchman.Discord.IntegrationTests.TestEnvironment.FakeDatabases;
using Watchman.IoC;

namespace Watchman.Discord.IntegrationTests.TestEnvironment
{
    internal class TestWatchmanBotFactory
    {
        public BotCommandsRunner CreateCommandsRunner()
        {
            var configuration = new DiscordConfiguration();

            var builder = new ContainerBuilder();
            builder.RegisterInstance(() => new FakeDiscordClient())
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterInstance(() => new FakeSessionFactory())
                .AsImplementedInterfaces()
                .SingleInstance();

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
