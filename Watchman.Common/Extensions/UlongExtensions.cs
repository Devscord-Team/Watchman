namespace Watchman.Common.Extensions
{
    public static class UlongExtensions
    {
        public static string GetUserMention(this ulong userId)
        {
            return $"<@{userId}>";
        }

        public static string GetChannelMention(this ulong channelId)
        {
            return $"<#{channelId}>"; 
        }

        public static string GetRoleMention(this ulong roleId)
        {
            return $"<@&{roleId}>";
        }
    }
}
