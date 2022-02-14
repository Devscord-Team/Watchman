using Autofac;
using Devscord.DiscordFramework.Integration.Services.Interfaces;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Discord.IntegrationTests.TestEnvironment.FakeClients;
using Watchman.Discord.IntegrationTests.TestEnvironment.FakeDatabases;
using Watchman.Discord.IntegrationTests.TestEnvironment.Models;
using Watchman.Integrations.Database;
using Watchman.IoC;

namespace Watchman.Discord.IntegrationTests.TestEnvironment
{
    internal class TestWatchmanBotFactory
    {
        public BotCommandsRunner CreateCommandsRunner()
        {
            var configuration = new DiscordConfiguration();

            var builder = new ContainerBuilder();
            builder.RegisterType<FakeSessionFactory>().As<ISessionFactory>().SingleInstance().PreserveExistingDefaults();

            builder.RegisterType<FakeDiscordClientMessagesService>().As<IDiscordClientMessagesService>().SingleInstance();
            builder.RegisterType<FakeDiscordClientUsersService>().As<IDiscordClientUsersService>().SingleInstance();
            builder.RegisterType<FakeDiscordClientChannelsService>().As<IDiscordClientChannelsService>().SingleInstance();
            builder.RegisterType<FakeDiscordClientRolesService>().As<IDiscordClientRolesService>().SingleInstance();
            builder.RegisterType<FakeDiscordClientServersService>().As<IDiscordClientServersService>().SingleInstance();
            builder.RegisterType<FakeDiscordClient>().As<IDiscordClient>().SingleInstance().PreserveExistingDefaults();

            var container = new ContainerModule();
            builder = container.FillBuilder(builder);
            var context = builder.Build();

            var workflowBuilder = new WatchmanBot(configuration, context)
                .GetWorkflowBuilder(useDiscordNetClient: false);
            workflowBuilder.Build();

            var client = context.Resolve<IDiscordClient>();
            return new BotCommandsRunner(client);
        }
    }

    public class BotCommandsRunner
    {
        private readonly IDiscordClient client;

        public BotCommandsRunner(IDiscordClient client)
        {
            this.client = client;
        }

        public async Task SendMessage(string text)
        {
            var message = new FakeMessage
            {
                Content = text
            };
            await client.MessagesService.MessageReceived.Invoke(message);
        }
    }
}
