using Autofac;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Architecture.Controllers;
using Devscord.DiscordFramework.Commands;
using Devscord.DiscordFramework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Commands.Services;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Newtonsoft.Json;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Devscord.DiscordFramework
{
    public interface IControllersService
    {
        Task Run(ulong messageId, DiscordRequest request, Contexts contexts);
    }
    //todo testy wydajnościowe
    internal class ControllersService : IControllersService
    {
        private readonly IComponentContext _context;
        private readonly Assembly controllersAssembly;
        private readonly IBotCommandsService _botCommandsService;
        private readonly ICommandsContainer _commandsContainer;
        private ControllersContainer controllersContainer;
        public ControllersService(IComponentContext context, Assembly controllersAssembly, IBotCommandsService botCommandsService,
            ICommandsContainer commandsContainer)
        {
            this._context = context;
            this.controllersAssembly = controllersAssembly;
            this._botCommandsService = botCommandsService;
            this._commandsContainer = commandsContainer;
        }

        public async Task Run(ulong messageId, DiscordRequest request, Contexts contexts)
        {
            if (this.controllersContainer == null)
            {
                this.LoadControllers();
            }
            using var messageIdHandler = LogContext.PushProperty("MessageId", contexts.Message.MessageId);
            using var recognizedAsCommandHandler = LogContext.PushProperty("RecognizedAsCommand", request.IsCommandForBot);
            Log.Information("Start processing request {message} that is recognized as command {isCommand} from user {userId} on channel {channelId} on server {serverId}", 
                request.OriginalMessage, 
                request.IsCommandForBot,
                contexts.User.Id,
                contexts.Channel.Id,
                contexts.Server.Id);

            Task readAlwaysTask = null;
            Task commandsTask = null;
            Task botCommandsTask = null;

            

            var readAlwaysMethods = this.controllersContainer.WithReadAlways;
            if(readAlwaysMethods.Any())
            {
                readAlwaysTask = Task.Run(() => 
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    this.RunMethods(request, contexts, readAlwaysMethods, true);
                    stopwatch.Stop();
                    Log.Information("Elapsed time {elapsedReadAlwaysMS}ms for ReadAlways}", 
                        stopwatch.ElapsedMilliseconds, request.OriginalMessage, request.IsCommandForBot);
                });
            }
            if (request.IsCommandForBot)
            {
                var discordCommandMethods = this.controllersContainer.WithDiscordCommand;
                if(discordCommandMethods.Any())
                {
                    commandsTask = Task.Run(() => 
                    {
                        var stopwatch = new Stopwatch();
                        stopwatch.Start();
                        this.RunMethods(request, contexts, discordCommandMethods, false);
                        stopwatch.Stop();
                        Log.Information("Elapsed time {elapsedDiscordCommandMS}ms} for DiscordRequest",
                            stopwatch.ElapsedMilliseconds, request.OriginalMessage, request.IsCommandForBot);
                    });
                }
                var discordBotCommandMethods = this.controllersContainer.WithIBotCommand;
                if (discordBotCommandMethods.Any())
                {
                    botCommandsTask = Task.Run(async () => 
                    {
                        var stopwatch = new Stopwatch();
                        stopwatch.Start();
                        await this.RunMethodsIBotCommand(request, contexts, discordBotCommandMethods);
                        stopwatch.Stop();
                        Log.Information("Elapsed time {elapsedIBotCommandMS}ms} for IBotCommand",
                            stopwatch.ElapsedMilliseconds, request.OriginalMessage, request.IsCommandForBot);
                    });
                }
            }

            // ReadAlwaysMethods should be first in throwing exception, bcs every ReadAlways exception is Error
            if(readAlwaysTask != null)
            {
                await readAlwaysTask;
            }
            if (commandsTask != null)
            {
                await commandsTask;
            }
            if (botCommandsTask != null)
            {
                await botCommandsTask;
            }
        }

        private void LoadControllers()
        {
            var controllers = this.controllersAssembly.GetTypesByInterface<IController>()
                .Select(x => new ControllerInfo((IController) this._context.Resolve(x)))
                .ToArray();
            this.controllersContainer = new ControllersContainer(controllers);
        }

        private void RunMethods(DiscordRequest request, Contexts contexts, ControllerInfo[] controllers, bool isReadAlways)
        {
            var tasks = new List<Task>();
            foreach (var controllerInfo in controllers)
            {
                foreach (var method in controllerInfo.Methods)
                {
                    if (isReadAlways)
                    {
                        Log.Information("Invoking command {invokedCommand}", method.Name);
                        var task = InvokeMethod(request, contexts, controllerInfo, method);
                        tasks.Add(task);
                        continue;
                    }

                    var command = method.GetAttributeInstances<DiscordCommand>();
                    if (this.IsMatchedCommand(command, request) && this.IsValid(contexts, method))
                    {
                        Log.Information("Invoking command {invokedCommand}", command.First().Command);
                        var task = InvokeMethod(request, contexts, controllerInfo, method);
                        tasks.Add(task);
                    }
                }
            }
            Task.WaitAll(tasks.ToArray());
        }

        private async Task RunMethodsIBotCommand(DiscordRequest request, Contexts contexts, ControllerInfo[] controllers)
        {
            if (!request.IsCommandForBot)
            {
                return;
            }

            foreach (var controllerInfo in controllers)
            {
                foreach (var method in controllerInfo.Methods)
                {
                    var commandInParameterType = method.GetParameters().First(x => typeof(IBotCommand).IsAssignableFrom(x.ParameterType)).ParameterType;
                    //TODO zoptymalizować, spokojnie można to pobierać wcześniej i używać raz, zamiast wszystko obliczać przy każdym odpaleniu
                    var template = this._botCommandsService.GetCommandTemplate(commandInParameterType);
                    var customCommand = await this._commandsContainer.GetCommand(request, commandInParameterType, contexts.Server.Id);
                    var isCommandMatchedWithCustom = customCommand != null;
                    var isThereDefaultCommandWithGivenName = request.Name.ToLowerInvariant() == template.NormalizedCommandName;
                    if (!isCommandMatchedWithCustom && !isThereDefaultCommandWithGivenName)
                    {
                        continue;
                    }
                    if (!this.IsValid(contexts, method))
                    { 
                        return;
                    }
                    var command = this.CreateBotCommand(isThereDefaultCommandWithGivenName, template, commandInParameterType, request, customCommand?.Template, isCommandMatchedWithCustom);
                    Log.Information("Invoking command {invokedCommand}", command.GetType().Name);
                    await InvokeMethod(command, contexts, controllerInfo, method);
                }
            }
        }

        private bool IsValid(Contexts contexts, MethodInfo method)
        {
            if(!this.CheckPermissions(method, contexts))
            {
                throw new NotAdminPermissionsException();
            }
            return true;
        }

        private IBotCommand CreateBotCommand(bool isThereDefaultCommandWithGivenName, BotCommandTemplate template, Type commandInParameterType, DiscordRequest request, Regex customTemplate, bool isCommandMatchedWithCustom)
        {
            var isDefaultCommand = isThereDefaultCommandWithGivenName && this._botCommandsService.IsDefaultCommand(template, request.Arguments, isCommandMatchedWithCustom);
            if (isDefaultCommand && this._botCommandsService.AreDefaultCommandArgumentsCorrect(template, request.Arguments))
            {
                return this._botCommandsService.ParseRequestToCommand(commandInParameterType, request, template);
            }
            else if (isCommandMatchedWithCustom && this._botCommandsService.AreCustomCommandArgumentsCorrect(template, customTemplate, request.OriginalMessage))
            {
                return this._botCommandsService.ParseCustomTemplate(commandInParameterType, template, customTemplate, request.OriginalMessage);
            }
            else
            {
                throw new InvalidArgumentsException();
            }
        }

        private bool IsMatchedCommand(IEnumerable<DiscordCommand> commands, DiscordRequest request)
        {
            return commands.Any(x =>
            {
                var withoutPrefix = request.OriginalMessage.CutStart(request.Prefix);
                return withoutPrefix.StartsWith(x.Command)
                       && (withoutPrefix.Length == x.Command.Length || withoutPrefix[x.Command.Length] == ' ');
            });
        }

        private bool CheckPermissions(MethodInfo method, Contexts contexts)
        {
            if (method.HasAttribute<AdminCommand>() && !contexts.User.IsAdmin())
            {
                return false;
            }
            return true;
        }

        private static Task InvokeMethod(DiscordRequest request, Contexts contexts, ControllerInfo controllerInfo, MethodInfo method)
            => SmartInvokeMethod(method, method => method.Invoke(controllerInfo.Controller, new object[] { request, contexts }));

        private static Task InvokeMethod(IBotCommand command, Contexts contexts, ControllerInfo controllerInfo, MethodInfo method)
            => SmartInvokeMethod(method, method => method.Invoke(controllerInfo.Controller, new object[] { command, contexts }));

        private static Task SmartInvokeMethod(MethodInfo method, Func<MethodInfo, object> methodInvoke)
            => methodInvoke.Invoke(method) is Task task ? task : Task.CompletedTask;
    }
}
