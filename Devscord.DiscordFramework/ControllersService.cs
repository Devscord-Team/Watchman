using Autofac;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Framework.Commands.Services;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework
{
    internal class ControllersService
    {
        private readonly IComponentContext _context;
        private readonly Assembly _assembly;
        private readonly BotCommandsService _botCommandsService;
        private readonly CommandsContainer _commandsContainer;
        private ControllersContainer _controllersContainer;

        public ControllersService(IComponentContext context, Assembly assembly, BotCommandsService botCommandsService,
            CommandsContainer commandsContainer)
        {
            this._context = context;
            this._assembly = assembly;
            this._botCommandsService = botCommandsService;
            this._commandsContainer = commandsContainer;
        }

        public async Task Run(DiscordRequest request, Contexts contexts)
        {
            if (this._controllersContainer == null)
            {
                this.LoadControllers();
            }

            using (LogContext.PushProperty("MessageId", Guid.NewGuid()))
            using (LogContext.PushProperty("Request", request))
            using (LogContext.PushProperty("Contexts", contexts))
            {
                var readAlwaysMethods = this._controllersContainer.WithReadAlways;
                var readAlwaysTask = Task.Run(() => RunMethods(request, contexts, readAlwaysMethods, true));

                Task commandsTask = null;
                Task botCommandsTask = null;
                if (request.IsCommandForBot)
                {
                    var discordCommandMethods = this._controllersContainer.WithDiscordCommand;
                    commandsTask = Task.Run(() => RunMethods(request, contexts, discordCommandMethods, false));

                    var discordBotCommandMethods = this._controllersContainer.WithIBotCommand;
                    botCommandsTask = Task.Run(() => RunMethodsIBotCommand(request, contexts, discordBotCommandMethods, false));
                }

                // ReadAlwaysMethods should be first in throwing exception, bcs every ReadAlways exception is Error
                await readAlwaysTask;
                if (commandsTask != null)
                {
                    await commandsTask;
                }
                if (botCommandsTask != null)
                {
                    await botCommandsTask;
                }
            }
        }

        private void LoadControllers()
        {
            var controllers = _assembly.GetTypesByInterface<IController>()
                .Select(x => (IController)_context.Resolve(x))
                .Select(x => new ControllerInfo(x))
                .ToList();
            this._controllersContainer = new ControllersContainer(controllers);
        }

        private void RunMethods(DiscordRequest request, Contexts contexts, IEnumerable<ControllerInfo> controllers, bool isReadAlways)
        {
            var tasks = new List<Task>();
            foreach (var controllerInfo in controllers)
            {
                using (LogContext.PushProperty("Controller", controllerInfo.Controller.GetType().Name))
                {
                    foreach (var method in controllerInfo.Methods)
                    {
                        if (isReadAlways)
                        {
                            var task = InvokeMethod(request, contexts, controllerInfo, method);
                            tasks.Add(task);
                            continue;
                        }

                        var command = method.GetAttributeInstances<DiscordCommand>();
                        if (IsMatchedCommand(command, request) && IsValid(contexts, method))
                        {
                            var task = InvokeMethod(request, contexts, controllerInfo, method);
                            tasks.Add(task);
                        }
                    }
                }
            }
            Task.WaitAll(tasks.ToArray());
        }

        private async Task RunMethodsIBotCommand(DiscordRequest request, Contexts contexts, IEnumerable<ControllerInfo> controllers, bool isReadAlways)
        {
            foreach (var controllerInfo in controllers)
            {
                using (LogContext.PushProperty("Controller", controllerInfo.Controller.GetType().Name))
                {
                    foreach (var method in controllerInfo.Methods)
                    {
                        if (!IsValid(contexts, method))
                        {
                            continue;
                        }
                        var commandInParameterType = method.GetParameters().First(x => typeof(IBotCommand).IsAssignableFrom(x.ParameterType)).ParameterType;
                        var template = this._botCommandsService.GetCommandTemplate(commandInParameterType); //TODO zoptymalizować, spokojnie można to pobierać wcześniej i używać raz, zamiast wszystko obliczać przy każdym odpaleniu
                        var isToContinue = false;
                        if (!this._botCommandsService.IsMatchedWithCommand(request, template))
                        {
                            isToContinue = true;
                        }
                        IBotCommand command;
                        if(isToContinue)
                        {
                            var customCommand = await this._commandsContainer.GetCommand(request, commandInParameterType);
                            if(customCommand == null)
                            {
                                continue;
                            }
                            command = this._botCommandsService.ParseCustomTemplate(commandInParameterType, template, customCommand.Template, request.OriginalMessage);
                        }
                        else
                        {
                            command = this._botCommandsService.ParseRequestToCommand(commandInParameterType, request, template);
                        }
                        await InvokeMethod(command, contexts, controllerInfo, method);
                    }
                }
            }
        }

        private bool IsValid(Contexts contexts, MethodInfo method)
        {
            CheckPermissions(method, contexts);
            return true;
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

        private void CheckPermissions(MethodInfo method, Contexts contexts)
        {
            Log.Information("Checking permissions for user {user} for method {method}", contexts.User.ToString(), method.Name);
            if (method.HasAttribute<AdminCommand>() && !contexts.User.IsAdmin)
            {
                throw new NotAdminPermissionsException();
            }
            Log.Information("User {user} have permissions for method {method}", contexts.User.ToString(), method.Name);
        }

        private static Task InvokeMethod(DiscordRequest request, Contexts contexts, ControllerInfo controllerInfo, MethodInfo method)
        {
            Log.Information("Invoke in controller {controller} method {method}", controllerInfo.Controller.GetType().Name, method.Name);

            using (LogContext.PushProperty("Method", method.Name))
            {
                var runningMethod = method.Invoke(controllerInfo.Controller, new object[] { request, contexts });
                if (runningMethod is Task task)
                {
                    return task;
                }
            }
            return Task.CompletedTask;
        }

        private static Task InvokeMethod(IBotCommand command, Contexts contexts, ControllerInfo controllerInfo, MethodInfo method)
        {
            Log.Information("Invoke in controller {controller} method {method}", controllerInfo.Controller.GetType().Name, method.Name);

            using (LogContext.PushProperty("Method", method.Name))
            {
                var runningMethod = method.Invoke(controllerInfo.Controller, new object[] { command, contexts });
                if (runningMethod is Task task)
                {
                    return task;
                }
            }
            return Task.CompletedTask;
        }
    }

    public class CommandsContainer
    {
        private readonly ICustomCommandsLoader _customCommandsLoader;
        private Dictionary<string, List<CustomCommand>> _customCommandsGroupedByBotCommand;
        private DateTime _lastRefresh;

        public CommandsContainer(ICustomCommandsLoader customCommandsLoader)
        {
            this._customCommandsLoader = customCommandsLoader;
        }

        public async Task<CustomCommand> GetCommand(DiscordRequest request, Type botCommand)
        {
            await this.TryRefresh();
            if(!this._customCommandsGroupedByBotCommand.ContainsKey(botCommand.FullName))
            {
                return null;
            }
            var command = this._customCommandsGroupedByBotCommand[botCommand.FullName].FirstOrDefault(x => x.Template.IsMatch(request.OriginalMessage));
            return command;
        }

        private async Task TryRefresh()
        {
            if(this._lastRefresh > DateTime.UtcNow.AddMinutes(-15))
            {
                return;
            }
            var customCommands = await this._customCommandsLoader.GetCustomCommands();
            this._customCommandsGroupedByBotCommand = customCommands.GroupBy(x => x.ExpectedBotCommandName).ToDictionary(k => k.Key, v => v.ToList());
            this._lastRefresh = DateTime.UtcNow;
        }
    }

    public class CustomCommand
    {
        public string ExpectedBotCommandName { get; private set; } //IBotCommand
        public Regex Template { get; private set; }

        public CustomCommand(string expectedBotCommandName, Regex template)
        {
            this.ExpectedBotCommandName = expectedBotCommandName;
            this.Template = template;
        }
    }

    public interface ICustomCommandsLoader
    {
        Task<List<CustomCommand>> GetCustomCommands();
    }
}
