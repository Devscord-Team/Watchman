using Autofac;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Framework.Commands.Parsing;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.WebSocket;
using Serilog;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Middlewares.Factories;
using System.Collections.Generic;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Discord.Rest;
using System.Diagnostics;
using Devscord.DiscordFramework.Framework.Commands.Services;
using Devscord.DiscordFramework.Integration;
using Discord;

namespace Devscord.DiscordFramework
{
    internal class Workflow
    {
        private readonly CommandParser _commandParser = new CommandParser();
        private readonly MiddlewaresService _middlewaresService = new MiddlewaresService();
        private readonly ControllersService _controllersService;
        private readonly Stopwatch _stopWatch = new Stopwatch();
        private readonly IComponentContext _context;

        public List<Func<Task>> OnReady { get; set; } = new List<Func<Task>>();
        public List<Func<Contexts, Task>> OnUserJoined { get; set; } = new List<Func<Contexts, Task>>();
        public List<Func<DiscordServerContext, Task>> OnDiscordServerAddedBot { get; set; } = new List<Func<DiscordServerContext, Task>>();
        public List<Func<SocketMessage, Task>> OnMessageReceived { get; set; } = new List<Func<SocketMessage, Task>>();
        public List<Action<Exception, Contexts>> OnWorkflowException { get; set; } = new List<Action<Exception, Contexts>>();
        public List<Action<ReactionContext>> OnUserAddedReaction { get; set; } = new List<Action<ReactionContext>>();
        public List<Action<ReactionContext>> OnUserRemovedReaction { get; set; } = new List<Action<ReactionContext>>();

        internal Workflow(Assembly botAssembly, IComponentContext context)
        {
            this._controllersService = new ControllersService(context, botAssembly, context.Resolve<BotCommandsService>(), context.Resolve<CommandsContainer>());
            this._context = context;
        }

        internal Workflow AddMiddleware<T>()
            where T : IMiddleware
        {
            this._middlewaresService.AddMiddleware<T>();
            Log.Debug("Added Middleware: {middlewareName}", nameof(T));
            return this;
        }

        internal void Initialize()
        {
            this.OnMessageReceived.Add(message => Task.Run(() => this.MessageReceived(message)));
        }

        internal void MapHandlers(DiscordSocketClient client)
        {
            this.OnReady.ForEach(x => client.Ready += x);
            this.OnMessageReceived.ForEach(x => client.MessageReceived += x);
            Server.UserJoined += CallUserJoined;
            Server.BotAddedToServer += CallServerAddedBot;
            client.ReactionAdded += GetReactionAddedFunc();
            client.ReactionRemoved += GetReactionRemovedFunc();
        }

        private Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> GetReactionAddedFunc()
        {
            return (userMessage, socketMessageChannel, socketReaction) => Task.Run(() =>
            {
                var reactionContext = _context.Resolve<ReactionContextFactory>().Create((socketReaction, userMessage.GetOrDownloadAsync().Result));
                OnUserAddedReaction.ForEach(x => x.Invoke(reactionContext));
            });
        }

        private Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> GetReactionRemovedFunc()
        {
            return (userMessage, socketMessageChannel, socketReaction) => Task.Run(() =>
            {
                var reactionContext = _context.Resolve<ReactionContextFactory>().Create((socketReaction, userMessage.GetOrDownloadAsync().Result));
                OnUserRemovedReaction.ForEach(x => x.Invoke(reactionContext));
            });
        }

        private async void MessageReceived(SocketMessage socketMessage)
        {
            if (ShouldIgnoreMessage(socketMessage))
            {
                return;
            }

            DiscordRequest request;
            Contexts contexts;
            try
            {
                request = this.ParseRequest(socketMessage);
                contexts = this.GetContexts(socketMessage);
            }
            catch (Exception e)
            {
                Log.Error(e, e.StackTrace);
                return;
            }
            try
            {
                Log.Information("Starting controllers");
                await this._controllersService.Run(request, contexts);
            }
            catch (Exception e)
            {
                Log.Error(e, e.StackTrace);
                OnWorkflowException.ForEach(x => x.Invoke(e, contexts));
            }
            var elapsedRun = this._stopWatch.ElapsedTicks;
            Log.Information("_controllersService.Run time {elapsedRun}ticks", elapsedRun);
#if DEBUG
            await socketMessage.Channel.SendMessageAsync($"```Run time: {elapsedRun}ticks```");
#endif
            this._stopWatch.Stop();
            this._stopWatch.Reset();
        }

        private DiscordRequest ParseRequest(SocketMessage socketMessage)
        {
            this._stopWatch.Restart();
            Log.Information("Processing message: {content} from user {user} started", socketMessage.Content, socketMessage.Author);
            var request = _commandParser.Parse(socketMessage.Content, socketMessage.Timestamp.UtcDateTime);
            var elapsedParse = this._stopWatch.ElapsedTicks;
            Log.Information("Parsing time: {elapsedParse}ticks", elapsedParse);
#if DEBUG
            socketMessage.Channel.SendMessageAsync($"```Parsing time: {elapsedParse}ticks```").Wait();
#endif
            Log.Information("Request parsed");
            return request;
        }

        private Contexts GetContexts(SocketMessage socketMessage)
        {
            this._stopWatch.Restart();
            var contexts = this._middlewaresService.RunMiddlewares(socketMessage);
            var elapsedMiddlewares = this._stopWatch.ElapsedTicks;
            Log.Information("Middlewares time: {elapsedMiddlewares}ticks", elapsedMiddlewares);
#if DEBUG
            socketMessage.Channel.SendMessageAsync($"```Middlewares time: {elapsedMiddlewares}ticks```").Wait();
#endif
            Log.Information("Contexts created");
            return contexts;
        }

        private bool ShouldIgnoreMessage(SocketMessage socketMessage)
        {
#if DEBUG
            if (!socketMessage.Channel.Name.Contains("test"))
            {
                return true;
            }
#endif
            if (socketMessage.Author.IsBot || socketMessage.Author.IsWebhook)
            {
                return true;
            }
            if (socketMessage.Channel.Name.Contains("logs"))
            {
                return true;
            }

            return false;
        }

        private async Task CallUserJoined(SocketGuildUser guildUser)
        {
            var userFactory = new UserContextsFactory();
            var serverFactory = new DiscordServerContextFactory();

            var userContext = userFactory.Create(guildUser);
            var guild = await Server.GetGuild(guildUser.Guild.Id);
            var discordServerContext = serverFactory.Create(guild);
            var landingChannel = discordServerContext.LandingChannel;

            var contexts = new Contexts();
            contexts.SetContext(userContext);
            contexts.SetContext(discordServerContext);
            if (landingChannel != null)
            {
                contexts.SetContext(landingChannel);
            }

            OnUserJoined.ForEach(x => x.Invoke(contexts));
        }

        private async Task CallServerAddedBot(SocketGuild guild)
        {
            var discordServerFactory = new DiscordServerContextFactory();
            var restGuild = await Server.GetGuild(guild.Id);
            var discordServer = discordServerFactory.Create(restGuild);

            OnDiscordServerAddedBot.ForEach(x => x.Invoke(discordServer));
        }
    }
}
