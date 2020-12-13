using Devscord.DiscordFramework.Commons.Exceptions;
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
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Devscord.DiscordFramework
{
    public class RunnerOfIBotCommandMethods
    {
        private readonly IBotCommandsService _botCommandsService;
        private readonly ICommandsContainer _commandsContainer;
        private readonly ICommandMethodValidator _commandMethodValidator;

        public RunnerOfIBotCommandMethods(IBotCommandsService botCommandsService, ICommandsContainer commandsContainer, ICommandMethodValidator commandMethodValidator)
        {
            this._botCommandsService = botCommandsService;
            this._commandsContainer = commandsContainer;
            this._commandMethodValidator = commandMethodValidator;
        }

        public async Task RunMethodsIBotCommand(DiscordRequest request, Contexts contexts, IEnumerable<ControllerInfo> controllers)
        {
            foreach (var controllerInfo in controllers)
            {
                using (LogContext.PushProperty("Controller", controllerInfo.Controller.GetType().Name))
                {
                    foreach (var method in controllerInfo.Methods)
                    {
                        var commandInParameterType = method.GetParameters().First(x => typeof(IBotCommand).IsAssignableFrom(x.ParameterType)).ParameterType;
                        //TODO zoptymalizować, spokojnie można to pobierać wcześniej i używać raz, zamiast wszystko obliczać przy każdym odpaleniu
                        var template = this._botCommandsService.GetCommandTemplate(commandInParameterType);
                        var customCommand = await this._commandsContainer.GetCommand(request, commandInParameterType, contexts);
                        var isCommandMatchedWithCustom = customCommand != null;
                        var isThereDefaultCommandWithGivenName = request.Name.ToLowerInvariant() == template.NormalizedCommandName;
                        if (!isCommandMatchedWithCustom && !isThereDefaultCommandWithGivenName)
                        {
                            continue;
                        }
                        if (!this._commandMethodValidator.IsValid(contexts, method))
                        {
                            return;
                        }
                        var command = this.CreateBotCommand(isThereDefaultCommandWithGivenName, template, commandInParameterType, request, customCommand?.Template, isCommandMatchedWithCustom);
                        await this.InvokeMethod(command, contexts, controllerInfo, method);
                    }
                }
            }
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

        private Task InvokeMethod(IBotCommand command, Contexts contexts, ControllerInfo controllerInfo, MethodInfo method)
        {
            Log.Information("Invoke in controller {controller} method {method}", controllerInfo.Controller.GetType().Name, method.Name);

            using (LogContext.PushProperty("Method", method.Name))
            {
                var x = method.GetParameters();
                var runningMethod = method.Invoke(controllerInfo.Controller, new object[] { command, contexts });
                if (runningMethod is Task task)
                {
                    return task;
                }
            }
            return Task.CompletedTask;
        }
    }
}
