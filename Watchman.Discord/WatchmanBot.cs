using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Middlewares;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Watchman.Discord.Areas.Protection.Controllers;
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
            _client.UserJoined += this.UserJoined;
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

        private Task UserJoined(SocketGuildUser user)
        {
            // todo: call method for welcoming user
            return Task.CompletedTask;
        }

        //private Task Log(LogMessage msg)
        //{
        //    return this._workflow.Run(msg);
        //}

        public void Dispose()
        {
            this._client.Dispose();
        }
    }
}
