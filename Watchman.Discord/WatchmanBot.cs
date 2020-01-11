using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Middlewares;
using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Services;
using Watchman.Integrations.MongoDB;
using Watchman.Discord.Ioc;
using Devscord.DiscordFramework;
using Autofac;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Discord.Areas.Help.Services;
using System.Text;
using System.Collections.Generic;
using Devscord.DiscordFramework.Commons.Extensions;

namespace Watchman.Discord
{
    public class WatchmanBot : IDisposable
    {
        private readonly DiscordSocketClient _client;
        private readonly Workflow _workflow;
        private readonly DiscordConfiguration _configuration;
        private readonly IContainer _container;

        public WatchmanBot(DiscordConfiguration configuration)
        {
            this._configuration = configuration;
            this._client = new DiscordSocketClient(new DiscordSocketConfig
            {
                TotalShards = 1
            });
            this._client.MessageReceived += this.MessageReceived;

            this._container = GetAutofacContainer(configuration);
            this._workflow = GetWorkflow(configuration, _container);

            var dataCollector = _container.Resolve<HelpDataCollectorService>();
            var helpService = _container.Resolve<HelpDBGeneratorService>();

            helpService.FillDatabase(dataCollector.GetCommandsInfo(typeof(WatchmanBot).Assembly));
        }

        public async Task Start()
        {
            MongoConfiguration.Initialize();
            ServerInitializer.Initialize(this._client);

            await _client.LoginAsync(TokenType.Bot, this._configuration.Token);
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private Task MessageReceived(SocketMessage message)
        {
#if DEBUG
            if (!message.Channel.Name.Contains("test"))
                return Task.CompletedTask;
#endif
            return message.Author.IsBot ? Task.CompletedTask : this._workflow.Run(message);
        }

        private void LogException(Exception e, Contexts contexts)
        {
            var messagesService = _container.Resolve<MessagesServiceFactory>().Create(contexts);

            if (e is NotAdminPermissionsException)
            {
                messagesService.SendResponse(x => x.UserIsNotAdmin(), contexts);
                return;
            }
            else
            {
                messagesService.SendMessage("Wystąpił nieznany wyjątek");
            }
#if DEBUG
            var lines = new Dictionary<string, string>()
            {
                { "Message", e.Message },
                { "InnerException message", e.InnerException?.Message },
                { "InnerException2 message", e.InnerException?.InnerException?.Message }
            };

            var exceptionMessageBuilder = new StringBuilder();
            exceptionMessageBuilder.PrintManyLines(lines);
            messagesService.SendMessage(exceptionMessageBuilder.ToString());
#endif
        }

        private Workflow GetWorkflow(DiscordConfiguration configuration, IContainer context)
        {
            var workflow = new Workflow(typeof(WatchmanBot).Assembly, context);
            workflow.AddMiddleware<ChannelMiddleware>()
                .AddMiddleware<ServerMiddleware>()
                .AddMiddleware<UserMiddleware>();
            workflow.WorkflowException += this.LogException;
            return workflow;
        }

        private IContainer GetAutofacContainer(DiscordConfiguration configuration)
        {
            return new ContainerModule(configuration)
                .GetBuilder()
                .Build();
        }

        public void Dispose()
        {
            this._client.Dispose();
        }
    }
}
