using Autofac;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Framework.Commands.Parsing;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Framework.Commands.Services;
using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Middlewares.Factories;
using Discord.WebSocket;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework
{
    internal class Workflow
    {
        private readonly IComponentContext _context;
        private readonly CommandParser _commandParser = new CommandParser();
        private readonly MiddlewaresService _middlewaresService = new MiddlewaresService();
        private readonly ControllersService _controllersService;
        private readonly Stopwatch _stopWatch = new Stopwatch();

        public List<Func<Task>> OnReady { get; set; } = new List<Func<Task>>();
        public List<Func<Contexts, Task>> OnUserJoined { get; set; } = new List<Func<Contexts, Task>>();
        public List<Func<DiscordServerContext, Task>> OnDiscordServerAddedBot { get; set; } = new List<Func<DiscordServerContext, Task>>();
        public List<Func<ChannelContext, DiscordServerContext, Task>> OnChannelCreated { get; set; } = new List<Func<ChannelContext, DiscordServerContext, Task>>();
        public List<Func<UserRole, UserRole, Task>> OnRoleUpdated { get; set; } = new List<Func<UserRole, UserRole, Task>>();
        public List<Func<UserRole, Task>> OnRoleCreated { get; set; } = new List<Func<UserRole, Task>>();
        public List<Func<UserRole, Task>> OnRoleRemoved { get; set; } = new List<Func<UserRole, Task>>();
        public List<Func<SocketMessage, Task>> OnMessageReceived { get; set; } = new List<Func<SocketMessage, Task>>();
        public List<Action<Exception, Contexts>> OnWorkflowException { get; set; } = new List<Action<Exception, Contexts>>();

        internal Workflow(Assembly botAssembly, IComponentContext context)
        {
            this._context = context;
            this._controllersService = new ControllersService(context, botAssembly, context.Resolve<BotCommandsService>(), context.Resolve<CommandsContainer>());
        }

        internal Workflow AddMiddleware<T>() where T : IMiddleware
        {
            this._middlewaresService.AddMiddleware<T>();
            Log.Debug("Added Middleware: {middlewareName}", nameof(T));
            return this;
        }

        internal void Initialize()
        {
            this.OnMessageReceived.Add(message => Task.Run(() =>
            {
                try
                {
                    this.MessageReceived(message);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, ex.StackTrace);
                }
            }));
        }

        internal void MapHandlers(DiscordSocketClient client)
        {
            this.OnReady.ForEach(x => client.Ready += x);
            this.OnMessageReceived.ForEach(x => client.MessageReceived += x);
            Server.UserJoined += this.CallUserJoined;
            Server.BotAddedToServer += this.CallServerAddedBot;
            Server.ChannelCreated += this.CallChannelCreated;
            Server.RoleRemoved += this.CallRoleRemoved;
            Server.RoleCreated += this.CallRoleCreated;
            Server.RoleUpdated += this.CallRoleUpdated;
            Log.Debug("Handlers have been mapped");
        }

        private async void MessageReceived(SocketMessage socketMessage)
        {
            if (this.ShouldIgnoreMessage(socketMessage))
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
                await this._controllersService.Run(socketMessage.Id, request, contexts);
            }
            catch (Exception e)
            {
                Log.Error(e, e.StackTrace);
                this.OnWorkflowException.ForEach(x => x.Invoke(e, contexts));
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
            var request = this._commandParser.Parse(socketMessage.Content, socketMessage.Timestamp.UtcDateTime);
            var elapsedParse = this._stopWatch.ElapsedTicks;
            Log.Information("Parsing time: {elapsedParse}ticks", elapsedParse);
#if DEBUG
            socketMessage.Channel.SendMessageAsync($"```Parsing time: {elapsedParse}ticks```").Wait();
#endif
            Log.Information("Request parsed {request}", request.ToJson());
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
            Log.Information("Contexts created {contexts}", contexts.ToJson());
            return contexts;
        }

        private bool ShouldIgnoreMessage(SocketMessage socketMessage)
        {
#if DEBUG
            if (!socketMessage.Channel.Name.Contains("test"))
            {
                Log.Information("MESSAGE {message}\nSKIPPED BECAUSE THIS IS NOT TEST CHANNEL - try to write it on channel with \"test\" word in name\nFor example \"test-1\"", socketMessage.Content);
                return true;
            }
#endif
            if (socketMessage.Author.IsBot || socketMessage.Author.IsWebhook)
            {
                Log.Debug("Message {message} skipped because is from bot or webhook", socketMessage.Content);
                return true;
            }
            if (socketMessage.Channel.Name.Contains("logs"))
            {
                Log.Debug("Message {message} skipped because is from logs channel", socketMessage.Content);
                return true;
            }

            return false;
        }

        private async Task CallUserJoined(SocketGuildUser guildUser)
        {
            var userContext = this._context.Resolve<UserContextsFactory>().Create(guildUser);
            var guild = await Server.GetGuild(guildUser.Guild.Id);
            var discordServer = this._context.Resolve<DiscordServerContextFactory>().Create(guild);
            var landingChannel = discordServer.LandingChannel;

            var contexts = new Contexts();
            contexts.SetContext(userContext);
            contexts.SetContext(discordServer);
            if (landingChannel != null)
            {
                contexts.SetContext(landingChannel);
            }
            Log.Information("User joined to server {contexts}", contexts.ToJson());
            this.OnUserJoined.ForEach(x => x.Invoke(contexts));
        }

        private async Task CallServerAddedBot(SocketGuild guild)
        {
            var restGuild = await Server.GetGuild(guild.Id);
            var discordServer = this._context.Resolve<DiscordServerContextFactory>().Create(restGuild);
            Log.Information("Bot added to server {server} with owner: {owner}", discordServer.ToJson(), guild.Owner.ToString());
            this.OnDiscordServerAddedBot.ForEach(x => x.Invoke(discordServer));
        }

        private async Task CallChannelCreated(SocketChannel socketChannel)
        {
            var channel = this._context.Resolve<ChannelContextFactory>().Create(socketChannel);
            Log.Information("Channel has been created {channel}", channel.ToJson());
            var guildChannel = await Server.GetGuildChannel(socketChannel.Id);
            var discordServerFactory = this._context.Resolve<DiscordServerContextFactory>();
            var guild = await Server.GetGuild(guildChannel.GuildId); // must get guild by id (not from guildChannel.Guild) - in opposite way it won't work
            var server = discordServerFactory.Create(guild);

            this.OnChannelCreated.ForEach(x => x.Invoke(channel, server));
        }

        private Task CallRoleUpdated(SocketRole from, SocketRole to)
        {
            var roleFactory = this._context.Resolve<UserRoleFactory>();
            var fromRole = roleFactory.Create(from);
            var toRole = roleFactory.Create(to);
            Log.Information("Role has been updated from {fromRole} to {toRole}", fromRole.ToJson(), toRole.ToJson());

            this.OnRoleUpdated.ForEach(x => x.Invoke(fromRole, toRole));
            return Task.CompletedTask;
        }

        private Task CallRoleCreated(SocketRole role)
        {
            var userRole = this._context.Resolve<UserRoleFactory>().Create(role);
            Log.Information("Role has been created {role}", userRole.ToJson());
            this.OnRoleCreated.ForEach(x => x.Invoke(userRole));
            return Task.CompletedTask;
        }

        private Task CallRoleRemoved(SocketRole role)
        {
            var userRole = this._context.Resolve<UserRoleFactory>().Create(role);
            Log.Information("Role has been removed {role}", userRole.ToJson());
            this.OnRoleRemoved.ForEach(x => x.Invoke(userRole));
            return Task.CompletedTask;
        }
    }
}
