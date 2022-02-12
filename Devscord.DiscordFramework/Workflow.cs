using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Architecture.Middlewares;
using Devscord.DiscordFramework.Commands.Parsing;
using Devscord.DiscordFramework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Commands.Services;
using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Middlewares.Factories;
using Discord;
using Discord.WebSocket;
using Serilog;

namespace Devscord.DiscordFramework
{
    //todo testy wydajnościowe
    internal class Workflow
    {
        private readonly CommandParser _commandParser;
        private readonly MiddlewaresService _middlewaresService;
        private readonly ControllersService _controllersService;
        private readonly DiscordServerContextFactory _discordServerContextFactory;
        private readonly UserContextsFactory _userContextsFactory;
        private readonly ChannelContextFactory _channelContextFactory;
        private readonly UserRoleFactory _userRoleFactory;
        private readonly Stopwatch _stopWatch = new Stopwatch();

        public List<Func<Task>> OnReady { get; set; } = new List<Func<Task>>();
        public List<Func<Contexts, Task>> OnUserJoined { get; set; } = new List<Func<Contexts, Task>>();
        public List<Func<DiscordServerContext, Task>> OnDiscordServerAddedBot { get; set; } = new List<Func<DiscordServerContext, Task>>();
        public List<Func<ChannelContext, DiscordServerContext, Task>> OnChannelCreated { get; set; } = new List<Func<ChannelContext, DiscordServerContext, Task>>();
        public List<Func<ChannelContext, DiscordServerContext, Task>> OnChannelRemoved { get; set; } = new List<Func<ChannelContext, DiscordServerContext, Task>>();
        public List<Func<UserRole, UserRole, Task>> OnRoleUpdated { get; set; } = new List<Func<UserRole, UserRole, Task>>();
        public List<Func<UserRole, Task>> OnRoleCreated { get; set; } = new List<Func<UserRole, Task>>();
        public List<Func<UserRole, Task>> OnRoleRemoved { get; set; } = new List<Func<UserRole, Task>>();
        public List<Func<SocketMessage, Task>> OnMessageReceived { get; set; } = new List<Func<SocketMessage, Task>>();
        public List<Func<Exception, DiscordRequest, Contexts, Task>> OnWorkflowException { get; set; } = new List<Func<Exception, DiscordRequest, Contexts, Task>>();

        internal Workflow(Assembly botAssembly, IComponentContext context)
        {
            this._controllersService = new ControllersService(context, botAssembly, context.Resolve<BotCommandsService>(), context.Resolve<CommandsContainer>());
            this._commandParser = context.Resolve<CommandParser>();
            this._middlewaresService = context.Resolve<MiddlewaresService>();
            this._discordServerContextFactory = context.Resolve<DiscordServerContextFactory>();
            this._userContextsFactory = context.Resolve<UserContextsFactory>();
            this._channelContextFactory = context.Resolve<ChannelContextFactory>();
            this._userRoleFactory = context.Resolve<UserRoleFactory>();
        }

        internal Workflow AddMiddleware<T>() where T : IMiddleware
        {
            this._middlewaresService.AddMiddleware<T>();
            Log.Debug("Added Middleware: {middlewareName}", nameof(T));
            return this;
        }

        internal void MapHandlers(DiscordSocketClient client)
        {
            this.OnReady.ForEach(x => client.Ready += () => this.WithExceptionHandlerAwait(x));
            this.OnMessageReceived.Add(message =>
            {
                _ = this.WithExceptionHandlerAwait(this.MessageReceived, message);
                return Task.CompletedTask;
            });
            this.OnMessageReceived.ForEach(func => client.MessageReceived += message =>
            {
                _ = this.WithExceptionHandlerAwait(func, message);
                return Task.CompletedTask;
            });

            Server.UserJoined += user => this.WithExceptionHandlerAwait(this.CallUserJoined, user);
            Server.BotAddedToServer += guild => this.WithExceptionHandlerAwait(this.CallServerAddedBot, guild);
            Server.ChannelCreated += channel => this.WithExceptionHandlerAwait(this.CallServerChannelCreated, channel);
            Server.ChannelRemoved += channel => this.WithExceptionHandlerAwait(this.CallServerChannelRemoved, channel);
            Server.RoleRemoved += role => this.WithExceptionHandlerAwait(this.CallRoleRemoved, role);
            Server.RoleCreated += role => this.WithExceptionHandlerAwait(this.CallRoleCreated, role);
            Server.RoleUpdated += (from, to) => this.WithExceptionHandlerAwait(this.CallRoleUpdated, from, to);
            Log.Debug("Handlers have been mapped");
        }

        private Task WithExceptionHandlerAwait(Func<Task> func)
        {
            var task = func.Invoke();
            return this.TryToAwaitTask(task);
        }

        private Task WithExceptionHandlerAwait<T>(Func<T, Task> func, T arg1)
        {
            var task = func.Invoke(arg1);
            return this.TryToAwaitTask(task);
        }
        
        private Task WithExceptionHandlerAwait<T, W>(Func<T, W, Task> func, T arg1, W arg2)
        {
            var task = func.Invoke(arg1, arg2);
            return this.TryToAwaitTask(task);
        }

        private async Task TryToAwaitTask(Task task, DiscordRequest request = null, Contexts sendExceptionsContexts = null)
        {
            try
            {
                await task;
            }
            catch (Exception e)
            {
                Log.Error(e, e.StackTrace);
                this.OnWorkflowException.ForEach(x => x.Invoke(e, request, sendExceptionsContexts));
            }
        }

        private async Task MessageReceived(SocketMessage socketMessage)
        {
            if (this.ShouldIgnoreMessage(socketMessage))
            {
                return;
            }
            var contexts = this.GetContexts(socketMessage);
            var request = this.ParseRequest(socketMessage);

            Log.Information("Starting controllers");
            await this.TryToAwaitTask(this._controllersService.Run(socketMessage.Id, request, contexts), request, contexts);

            var elapsedRun = this._stopWatch.ElapsedTicks;
            var elapsedMilliseconds = this._stopWatch.ElapsedMilliseconds;
            Log.Information("_controllersService.Run time {elapsedRun}ticks (ms: {milliseconds})", elapsedRun, elapsedMilliseconds);
#if DEBUG
            await socketMessage.Channel.SendMessageAsync($"```Run time: {elapsedRun}ticks (ms: {elapsedMilliseconds})```");
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
            var elapsedMilliseconds = this._stopWatch.ElapsedMilliseconds;
            _ = socketMessage.Channel.SendMessageAsync($"```Parsing time: {elapsedParse}ticks (ms: {elapsedMilliseconds})```");
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
            var elapsedMilliseconds = this._stopWatch.ElapsedMilliseconds;
            _ = socketMessage.Channel.SendMessageAsync($"```Middlewares time: {elapsedMiddlewares}ticks (ms: {elapsedMilliseconds})```");
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
            var userContext = this._userContextsFactory.Create(guildUser);
            var guild = await Server.GetGuild(guildUser.Guild.Id);
            var discordServer = this._discordServerContextFactory.Create(guild);
            var landingChannel = discordServer.LandingChannel;

            var contexts = new Contexts(discordServer, landingChannel, userContext);
            Log.Information("User joined to server {contexts}", contexts.ToJson());
            this.OnUserJoined.ForEach(x => x.Invoke(contexts));
        }

        private async Task CallServerAddedBot(SocketGuild guild)
        {
            var restGuild = await Server.GetGuild(guild.Id);
            var discordServer = this._discordServerContextFactory.Create(restGuild);
            Log.Information("Bot added to server {server} with owner: {owner}", discordServer.ToJson(), guild.Owner.ToString());
            this.OnDiscordServerAddedBot.ForEach(x => x.Invoke(discordServer));
        }

        private async Task CallServerChannelCreated(SocketChannel socketChannel)
        {
            if (!(socketChannel is IGuildChannel))
            {
                return;
            }
            var channel = this._channelContextFactory.Create(socketChannel);
            Log.Information("Channel has been created {channel}", channel.ToJson());
            var guildChannel = await Server.GetGuildChannel(socketChannel.Id);
            var guild = await Server.GetGuild(guildChannel.GuildId); // must get guild by id (not from guildChannel.Guild) - in opposite way it won't work
            var server = this._discordServerContextFactory.Create(guild);

            Task.WaitAll(this.OnChannelCreated.Select(x => x.Invoke(channel, server)).ToArray());
        }

        private Task CallServerChannelRemoved(SocketChannel socketChannel)
        {
            var channel = this._channelContextFactory.Create(socketChannel);
            Log.Information("Channel has been removed {channel}", channel.ToJson());
            var guild = ((IGuildChannel)socketChannel).Guild;
            var server = this._discordServerContextFactory.Create(guild);

            Task.WaitAll(this.OnChannelRemoved.Select(x => x.Invoke(channel, server)).ToArray());
            return Task.CompletedTask;
        }

        private Task CallRoleUpdated(SocketRole from, SocketRole to)
        {
            var fromRole = this._userRoleFactory.Create(from);
            var toRole = this._userRoleFactory.Create(to);
            Log.Information("Role has been updated from {fromRole} to {toRole}", fromRole.ToJson(), toRole.ToJson());

            this.OnRoleUpdated.ForEach(x => x.Invoke(fromRole, toRole));
            return Task.CompletedTask;
        }

        private Task CallRoleCreated(SocketRole role)
        {
            var userRole = this._userRoleFactory.Create(role);
            Log.Information("Role has been created {role}", userRole.ToJson());
            this.OnRoleCreated.ForEach(x => x.Invoke(userRole));
            return Task.CompletedTask;
        }

        private Task CallRoleRemoved(SocketRole role)
        {
            var userRole = this._userRoleFactory.Create(role);
            Log.Information("Role has been removed {role}", userRole.ToJson());
            this.OnRoleRemoved.ForEach(x => x.Invoke(userRole));
            return Task.CompletedTask;
        }
    }
}
