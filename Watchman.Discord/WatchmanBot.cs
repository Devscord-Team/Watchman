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

namespace Watchman.Discord
{
    public class WatchmanBot : IDisposable
    {
        private DiscordSocketClient _client;
        private Workflow _workflow;
        private DiscordConfiguration _configuration;

        public WatchmanBot(DiscordConfiguration configuration = null)
        {
            var configPath = "config.json";
#if RELEASE
            configPath = "config-prod.json";
#endif
            this._configuration = configuration ?? JsonConvert
                .DeserializeObject<DiscordConfiguration>(File.ReadAllText(configPath));

            this._client = new DiscordSocketClient(new DiscordSocketConfig
            {
                TotalShards = 1
            });
            this._workflow = new Workflow(typeof(WatchmanBot).Assembly);
            _client.MessageReceived += this.MessageReceived;
            _workflow.WorkflowException += this.LogException;
            //_client.Log += this.Log;
        }

        public async Task Start()
        {
            MongoConfiguration.Initialize();
            ServerInitializer.Initialize(this._client, this._configuration.MongoDbConnectionString);

            this._workflow
                .AddMiddleware<ChannelMiddleware>()
                .AddMiddleware<ServerMiddleware>()
                .AddMiddleware<UserMiddleware>()
                .WithControllers();

            await _client.LoginAsync(TokenType.Bot, this._configuration.Token);

            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private Task MessageReceived(SocketMessage message)
        {
            if (message.Author.IsBot)
            {
                return Task.CompletedTask;
            }
            return this._workflow.Run(message);
        }

        //private Task Log(LogMessage msg)
        //{
        //    return this._workflow.Run(msg);
        //}

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

        public void Dispose()
        {
            this._client.Dispose();
        }
    }
}
