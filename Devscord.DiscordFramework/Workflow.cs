using Autofac;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Framework.Commands.Parsing;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.WebSocket;
using Serilog;
using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework
{
    public class Workflow : Delegates
    {
        private readonly CommandParser _commandParser = new CommandParser();
        private readonly MiddlewaresService _middlewaresService = new MiddlewaresService();
        private readonly ControllersService _controllersService;

        public Action<Exception, Contexts> WorkflowException { get; set; }

        public Workflow(Assembly botAssembly, IComponentContext context)
        {
            this._controllersService = new ControllersService(context, botAssembly);
        }

        public Workflow AddMiddleware<T, W>() 
            where T : IMiddleware<W>
            where W : IDiscordContext
        {
            this._middlewaresService.AddMiddleware<T, W>();
            Log.Debug("Added Middleware: {middlewareName} with DiscordContext: {contextName}", nameof(T), nameof(W));
            return this;
        }

        public async Task Run(SocketMessage socketMessage)
        {
            Log.Information("Processing message: {content} from user {user} started", socketMessage.Content, socketMessage.Author);
            var request = _commandParser.Parse(socketMessage.Content);
            var contexts = this._middlewaresService.RunMiddlewares(socketMessage);
            try
            {
                await this._controllersService.Run(request, contexts);
            }
            catch (Exception e)
            {
                Log.Error(e, e.StackTrace);
                WorkflowException.Invoke(e, contexts);
            }
        }

        public void LogOnChannel(string message, ulong channelId)
        {
            var channel = (ISocketMessageChannel)Server.GetChannel(channelId);
            message = new StringBuilder(message).FormatMessageIntoBlock("json").ToString();
            channel.SendMessageAsync(message);
        }
    }
}
