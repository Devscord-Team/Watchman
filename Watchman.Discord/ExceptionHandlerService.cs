using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Serilog;

namespace Watchman.Discord
{
    public class ExceptionHandlerService
    {
        public static DiscordConfiguration DiscordConfiguration { get; set; }

        private Contexts DebugServerContexts
        {
            get
            {
                if (this._debugServerContexts == null)
                {
                    var server = this._discordServersService.GetDiscordServerAsync(DiscordConfiguration.ExceptionServerId).GetAwaiter().GetResult();
                    var channel = server.GetTextChannels().FirstOrDefault(x => x.Id == DiscordConfiguration.ExceptionChannelId);
                    var contexts = new Contexts(server, channel, user: null);
                    this._debugServerContexts = contexts;
                }
                return this._debugServerContexts;
            }
        }

        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly DiscordServersService _discordServersService;
        private Contexts _debugServerContexts;

        public ExceptionHandlerService(MessagesServiceFactory messagesServiceFactory, DiscordServersService discordServersService)
        {
            this._messagesServiceFactory = messagesServiceFactory;
            this._discordServersService = discordServersService;
        }

        public Task LogException(Exception e)
        {
            Log.Error(e.ToString());
            return Task.CompletedTask;
        }

        public Task SendExceptionResponse(Exception e, Contexts contexts)
        {
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
            return messagesService.SendMessage(exceptionMessage, Devscord.DiscordFramework.Commons.MessageType.BlockFormatted);
        }

        private StringBuilder BuildExceptionMessage(Exception e)
        {
            return new StringBuilder($"{e.Message}\r\n\r\n{e.InnerException}\r\n\r\n{e.StackTrace}").FormatMessageIntoBlock();
        }
    }
}
