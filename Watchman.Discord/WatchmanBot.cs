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

namespace Watchman.Discord
{
    public class WatchmanBot : IDisposable
    {
        private DiscordSocketClient _client;
        private Workflow _workflow;
        private DiscordConfiguration _configuration;

        public WatchmanBot(DiscordConfiguration configuration)
        {
            this._client = GetDiscordClient();
            var autofacContainer = GetAutofacContainer(configuration);
            this._workflow = GetWorkflow(configuration, autofacContainer);
        }

        public async Task Start()
        {
            MongoConfiguration.Initialize();
            ServerInitializer.Initialize(this._client, this._configuration.MongoDbConnectionString);

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
                DefaultChannelId = socketMessage.Channel.Id
            };

            if (e is System.Reflection.TargetInvocationException)
            {
                messagesService.SendMessage("Bot nie ma wystarczających uprawnień do wykonania tej akcji");
            }
#if DEBUG
            messagesService.SendMessage($"```Komenda: {socketMessage.Content}```");
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

        private DiscordSocketClient GetDiscordClient()
        {
            var client = new DiscordSocketClient(new DiscordSocketConfig
            {
                TotalShards = 1
            });
            client.MessageReceived += this.MessageReceived;
            return client;
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
