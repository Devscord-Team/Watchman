using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System;
using System.Collections.Generic;

namespace Watchman.Discord.ResponsesManagers
{
    public static class MutingResponsesManager
    {
        public static string MutedUser(this IResponsesService responsesService, UserContext user, DateTime timeEnd)
        {
            return responsesService.ProcessResponse("MutedUser",
                new KeyValuePair<string, string>("user", user.Name), //todo: change to GetMention when message will be send as Embed
                new KeyValuePair<string, string>("timeEnd", timeEnd.ToLocalTimeString()));
        }

        public static string UnmutedUser(this IResponsesService responsesService, UserContext user)
        {
            return responsesService.ProcessResponse("UnmutedUser",
                new KeyValuePair<string, string>("user", user.Name));
        }

        public static string UnmutedUserForUser(this IResponsesService responsesService, UserContext user, DiscordServerContext server)
        {
            return responsesService.ProcessResponse("UnmutedUserForUser",
                new KeyValuePair<string, string>("user", user.Name),
                new KeyValuePair<string, string>("server", server.Name));
        }

        public static string ThereAreNoMutedUsers(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ThereAreNoMutedUsers");
        }

        public static string MutedUsersListSent(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("MutedUsersListSent");
        }
        public static string UserWasntMuted(this IResponsesService responsesService, UserContext user)
        {
            return responsesService.ProcessResponse("UserWasntMuted",
                new KeyValuePair<string, string>("user", user.Name));
        }
    }
}
