using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Serilog;
using Watchman.Discord.Areas.Help.Services;

namespace Watchman.Discord
{
    public interface IExceptionHandlerService
    {
        Task LogException(Exception e);
        Task SendExceptionResponse(Exception e, DiscordRequest request, Contexts contexts);
        Task SendExceptionToDebugServer(Exception e);
        Task PrintDebugExceptionInfo(Exception e, Contexts contexts);
    }

    public class ExceptionHandlerService : IExceptionHandlerService
    {
        private readonly IDiscordServersService _discordServersService;
        private readonly IMessagesServiceFactory _messagesServiceFactory;
        private readonly IHelpService _helpService;
        private Contexts _debugServerContexts;

        private Contexts DebugServerContexts
        {
            get
            {
                if (this._debugServerContexts != null)
                {
                    return this._debugServerContexts;
                }
                var server = this._discordServersService.GetDiscordServerAsync(DiscordConfiguration.ExceptionServerId).GetAwaiter().GetResult();
                var channel = server.GetTextChannel(DiscordConfiguration.ExceptionChannelId);
                var contexts = new Contexts(server, channel, null);
                this._debugServerContexts = contexts;
                return this._debugServerContexts;
            }
        }

        public static DiscordConfiguration DiscordConfiguration { get; set; }

        public ExceptionHandlerService(IMessagesServiceFactory messagesServiceFactory, IDiscordServersService discordServersService, IHelpService helpService)
        {
            this._messagesServiceFactory = messagesServiceFactory;
            this._discordServersService = discordServersService;
            this._helpService =  helpService;
        }

        public Task LogException(Exception e)
        {
            Log.Error(e.ToString());
            return Task.CompletedTask;
        }

        public Task SendExceptionResponse(Exception e, DiscordRequest request, Contexts contexts)
        {
            if (e is InvalidArgumentsException && request is not null)
            {
                return this.SendHelpWhenInvalidArguments(request.Name, contexts);
            }

            var messagesService = this._messagesServiceFactory.Create(contexts ?? this.DebugServerContexts);
            var mostInnerException = e.InnerException ?? e;
            while (mostInnerException.InnerException != null)
            {
                mostInnerException = mostInnerException.InnerException;
            }
            return mostInnerException is BotException botException
                ? messagesService.SendExceptionResponse(botException)
                : messagesService.SendMessage("Wystąpił nieznany wyjątek");
        }

        public Task SendExceptionToDebugServer(Exception e)
        {
            var isBotException = e.InnerException is BotException;
            if (isBotException && DiscordConfiguration.SendOnlyUnknownExceptionInfo)
            {
                return Task.CompletedTask;
            }
            return this.PrintDebugExceptionInfo(e, this.DebugServerContexts);
        }

        public Task PrintDebugExceptionInfo(Exception e, Contexts contexts)
        {
            if (contexts == null)
            {
                return Task.CompletedTask;
            }
            var exceptionMessage = this.BuildExceptionMessage(e).ToString();
            var messagesService = this._messagesServiceFactory.Create(contexts);
            return messagesService.SendMessage(exceptionMessage, MessageType.BlockFormatted);
        }

        private Task SendHelpWhenInvalidArguments(string commandName, Contexts contexts)
        {
            var helpInformations = this._helpService.GetHelpInformations(contexts);
            return this._helpService.PrintHelpForOneCommand(commandName, contexts, helpInformations);
        }

        private StringBuilder BuildExceptionMessage(Exception e)
        {
            var exceptionMessage = new StringBuilder(e.Message).FormatMessageIntoBlock();
            var innerException = new StringBuilder(e.InnerException?.Message).FormatMessageIntoBlock();
            var stackTrace = new StringBuilder(e.StackTrace).FormatMessageIntoBlock();
            return exceptionMessage.Append(innerException).Append(stackTrace);
        }
    }
}