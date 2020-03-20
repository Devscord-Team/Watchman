using System;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Serilog;

namespace Watchman.Discord
{
    public class ExceptionHandlerService
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;

        public ExceptionHandlerService(MessagesServiceFactory messagesServiceFactory)
        {
            _messagesServiceFactory = messagesServiceFactory;
        }

        public void LogException(Exception e, Contexts contexts)
        {
            var messagesService = _messagesServiceFactory.Create(contexts);

            var mostInnerException = e.InnerException ?? e;

            while (mostInnerException.InnerException != null)
            {
                mostInnerException = mostInnerException.InnerException;
            }

            Log.Error(mostInnerException.ToString());

            switch (mostInnerException)
            {
                case NotAdminPermissionsException _:
                    messagesService.SendResponse(x => x.UserIsNotAdmin(), contexts);
                    break;
                case RoleNotFoundException roleExc:
                    messagesService.SendResponse(x => x.RoleNotFound(roleExc.RoleName), contexts);
                    break;
                case UserDidntMentionedAnyUserToMuteException _:
                    messagesService.SendResponse(x => x.UserDidntMentionedAnyUserToMute(), contexts);
                    break;
                case UserNotFoundException notFoundExc:
                    messagesService.SendResponse(x => x.UserNotFound(notFoundExc.Mention), contexts);
                    break;
                case TimeCannotBeNegativeException _:
                    messagesService.SendResponse(x => x.TimeCannotBeNegative(), contexts);
                    break;
                case TimeIsTooBigException _:
                    messagesService.SendResponse(x => x.TimeIsTooBig(), contexts);
                    break;
                default:
                    messagesService.SendMessage("Wystąpił nieznany wyjątek");
                    break;
            }
        }
    }
}
