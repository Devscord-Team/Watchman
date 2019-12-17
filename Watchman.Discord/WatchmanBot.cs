using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Middlewares;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Services;
using Watchman.Integrations.MongoDB;
using Watchman.Discord.Ioc;
using Devscord.DiscordFramework;
using Autofac.Core.Registration;
using Autofac;
using Watchman.Discord.Areas.Statistics.Controllers;
using Watchman.Cqrs;
using Watchman.DomainModel.Users.Commands;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;

namespace Watchman.Discord
{
    public class WatchmanBot : IDisposable
    {
        private DiscordSocketClient _client;
        private Workflow _workflow;
        private DiscordConfiguration _configuration;

        public WatchmanBot(DiscordConfiguration configuration)
        {
            this._configuration = configuration;
            this._client = new DiscordSocketClient(new DiscordSocketConfig
            {
                TotalShards = 1
            });
            this._client.MessageReceived += this.MessageReceived;

            var autofacContainer = GetAutofacContainer(configuration);
            this._workflow = GetWorkflow(configuration, autofacContainer);

            var dbHelpGenerator = autofacContainer.Resolve<DbHelpGeneratorService>();
            dbHelpGenerator.GenerateDefaultHelpDB(typeof(WatchmanBot).Assembly);
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
            return message.Author.IsBot ? Task.CompletedTask : this._workflow.Run(message);
        }

        private void LogException(Exception e, SocketMessage socketMessage)
        {
            var messagesService = new MessagesService()
            {
                ChannelId = socketMessage.Channel.Id
            };

            if (e is System.Reflection.TargetInvocationException)
            {
                messagesService.SendMessage("Wystąpił wyjątek");
            }
#if DEBUG
            messagesService.SendMessage($"```Komenda: {socketMessage.Content}```");
            messagesService.SendMessage($"```Message: {e.Message}```");
            messagesService.SendMessage($"```InnerException message: {e.InnerException.Message}```");
            messagesService.SendMessage($"```InnerException2 message: {e.InnerException?.InnerException.Message}```");
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
