<<<<<<< HEAD
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Responses;
=======
﻿using System;
using Devscord.DiscordFramework.Commons.Exceptions;
>>>>>>> master
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Serilog;
using System;

namespace Watchman.Discord
{
    public class ExceptionHandlerService
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;

        public ExceptionHandlerService(MessagesServiceFactory messagesServiceFactory)
        {
            this._messagesServiceFactory = messagesServiceFactory;
        }

        public void LogException(Exception e, Contexts contexts)
        {
<<<<<<< HEAD
            var messagesService = this._messagesServiceFactory.Create(contexts);

=======
            var messagesService = _messagesServiceFactory.Create(contexts);
>>>>>>> master
            var mostInnerException = e.InnerException ?? e;
            while (mostInnerException.InnerException != null)
            {
                mostInnerException = mostInnerException.InnerException;
            }

            Log.Error(mostInnerException.ToString());
            var task = mostInnerException is BotException botException
                ? messagesService.SendExceptionResponse(botException)
                : messagesService.SendMessage("Wystąpił nieznany wyjątek");
            task.Wait();
        }
    }
}
