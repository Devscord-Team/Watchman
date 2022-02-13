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
        private readonly Assembly _assembly;
        private readonly BotCommandsService _botCommandsService;
        private readonly ICommandsContainer _commandsContainer;
        private ControllersContainer _controllersContainer;

        public ControllersService(IComponentContext context, Assembly assembly, BotCommandsService botCommandsService,
            ICommandsContainer commandsContainer)
        {
            this._context = context;
            this._assembly = assembly;
            this._botCommandsService = botCommandsService;
            this._commandsContainer = commandsContainer;
        }

        public async Task Run(ulong messageId, DiscordRequest request, Contexts contexts)
        {
            if (this._controllersContainer == null)
            {
                this.LoadControllers();
            }
            var readAlwaysMethods = this._controllersContainer.WithReadAlways;
            var readAlwaysTask = Task.Run(() => this.RunMethods(request, contexts, readAlwaysMethods, true));

            Task commandsTask = null;
            Task botCommandsTask = null;
            if (request.IsCommandForBot)
            {
                var discordCommandMethods = this._controllersContainer.WithDiscordCommand;
                commandsTask = Task.Run(() => this.RunMethods(request, contexts, discordCommandMethods, false));

                var discordBotCommandMethods = this._controllersContainer.WithIBotCommand;
                botCommandsTask = Task.Run(() => this.RunMethodsIBotCommand(request, contexts, discordBotCommandMethods));
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

        private void LoadControllers()
        {
            var controllers = this._assembly.GetTypesByInterface<IController>()
                .Select(x => new ControllerInfo((IController) this._context.Resolve(x)))
                .ToList();
            this._controllersContainer = new ControllersContainer(controllers);
        }

        private void RunMethods(DiscordRequest request, Contexts contexts, IEnumerable<ControllerInfo> controllers, bool isReadAlways)
        {
            var tasks = new List<Task>();
            foreach (var controllerInfo in controllers)
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
                    if (this.IsMatchedCommand(command, request) && this.IsValid(contexts, method))
                    {
                        var task = InvokeMethod(request, contexts, controllerInfo, method);
                        tasks.Add(task);
                    }
                }
            }
            Task.WaitAll(tasks.ToArray());
        }

        private async Task RunMethodsIBotCommand(DiscordRequest request, Contexts contexts, IEnumerable<ControllerInfo> controllers)
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
                    await InvokeMethod(command, contexts, controllerInfo, method);
                }
            }
        }

        private bool IsValid(Contexts contexts, MethodInfo method)
        {
            this.CheckPermissions(method, contexts);
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

        private void CheckPermissions(MethodInfo method, Contexts contexts)
        {
            if (method.HasAttribute<AdminCommand>() && !contexts.User.IsAdmin())
            {
                throw new NotAdminPermissionsException();
            }
        }

        private static Task InvokeMethod(DiscordRequest request, Contexts contexts, ControllerInfo controllerInfo, MethodInfo method)
            => SmartInvokeMethod(method, method => method.Invoke(controllerInfo.Controller, new object[] { request, contexts }));

        private static Task InvokeMethod(IBotCommand command, Contexts contexts, ControllerInfo controllerInfo, MethodInfo method)
            => SmartInvokeMethod(method, method => method.Invoke(controllerInfo.Controller, new object[] { command, contexts }));

        private static Task SmartInvokeMethod(MethodInfo method, Func<MethodInfo, object> methodInvoke)
            => methodInvoke.Invoke(method) is Task task ? task : Task.CompletedTask;
    }
}
