using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devscord.DiscordFramework.Commons.Extensions
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
            return $"@&{roleId}>";
        }
    }
}
