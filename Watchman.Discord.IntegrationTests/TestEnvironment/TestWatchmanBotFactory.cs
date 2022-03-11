using Autofac;
using Devscord.DiscordFramework;
using Devscord.DiscordFramework.Integration.Services.Interfaces;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Discord.Areas.UselessFeatures.Controllers;
using Watchman.Discord.Areas.Users.Controllers;
using Watchman.Discord.IntegrationTests.TestEnvironment.FakeClients;
using Watchman.Discord.IntegrationTests.TestEnvironment.FakeDatabases;
using Watchman.Integrations.Database;
using Watchman.IoC;

namespace Watchman.Discord.IntegrationTests.TestEnvironment
{
    internal class TestWatchmanBotFactory
    {
        public BotCommandsRunner CreateCommandsRunner(List<object> instancesToRegister = null)
        {
            var configuration = new DiscordConfiguration();
            var builder = new ContainerBuilder();

            instancesToRegister?.ForEach(x =>
            {
                builder.RegisterInstance(x)
                    .AsImplementedInterfaces()
                    .SingleInstance()
                    .PreserveExistingDefaults();
            });
            this.RegisterDefaults(builder);

            var container = new ContainerModule();
            builder = container.FillBuilder(builder);
            var context = builder.Build();

            var workflowBuilder = new WatchmanBot(configuration, context)
                .GetWorkflowBuilder(useDiscordNetClient: false)
                .AddOnWorkflowExceptionHandlers(builder => 
                {
                    builder.AddHandler((ex, r, c) => throw ex);
                });
            workflowBuilder.Build();

            var workflow = context.Resolve<IWorkflow>();
            workflow.OnReady.ForEach(x => x.Invoke().Wait());

            var client = context.Resolve<IDiscordClient>();
            return new BotCommandsRunner(client);
        }

        private void RegisterDefaults(ContainerBuilder builder)
        {
            builder.RegisterType<FakeSessionFactory>()
                .As<ISessionFactory>()
                .SingleInstance()
                .PreserveExistingDefaults();
            builder.RegisterType<FakeDiscordClientMessagesService>()
                .As<IDiscordClientMessagesService>()
                .SingleInstance()
                .PreserveExistingDefaults();
            builder.RegisterType<FakeDiscordClientUsersService>()
                .As<IDiscordClientUsersService>()
                .SingleInstance()
                .PreserveExistingDefaults();
            builder.RegisterType<FakeDiscordClientChannelsService>()
                .As<IDiscordClientChannelsService>()
                .SingleInstance()
                .PreserveExistingDefaults();
            builder.RegisterType<FakeDiscordClientRolesService>()
                .As<IDiscordClientRolesService>()
                .SingleInstance()
                .PreserveExistingDefaults();
            builder.RegisterType<FakeDiscordClientServersService>()
                .As<IDiscordClientServersService>()
                .SingleInstance()
                .PreserveExistingDefaults();
            builder.RegisterType<FakeDiscordClient>()
                .As<IDiscordClient>()
                .SingleInstance()
                .PreserveExistingDefaults();
        }
    }
}
