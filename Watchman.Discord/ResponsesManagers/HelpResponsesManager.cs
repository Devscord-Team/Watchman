using Devscord.DiscordFramework.Commands.Responses;

namespace Watchman.Discord.ResponsesManagers
{
    public static class HelpResponsesManager
    {
        public static string NoDefaultDescription(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("NoDefaultDescription");
        }

        public static string AvailableCommands(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("AvailableCommands");
        }

        public static string HereYouCanFindAvailableCommandsWithDescriptions(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("HereYouCanFindAvailableCommandsWithDescriptions");
        }

        public static string HowToUseCommand(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("HowToUseCommand");
        }

        public static string Example(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("Example");
        }

        public static string Type(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("Type");
        }

        public static string Parameters(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("Parameters");
        }

        public static string ExampleChannelMention(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ExampleChannelMention");
        }

        public static string ExampleUserMention(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ExampleUserMention");
        }

        public static string ExampleList(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ExampleList");
        }

        public static string ExampleSingleWord(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ExampleSingleWord");
        }

        public static string ExampleText(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("ExampleText");
        }
    }
}
