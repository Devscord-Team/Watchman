namespace Devscord.DiscordFramework.Framework.Commands.AntiSpam
{
    public interface IUserMessagesCounter
    {
        int UserMessagesCountToBeSafe { get; }
        int CountUserMessages(ulong userId, ulong serverId);
    }
}