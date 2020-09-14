using System;
using System.Globalization;

namespace Devscord.DiscordFramework.Commons.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToLocalTimeString(this DateTime dateTime)
        {
            var convertedDateTime = dateTime.Kind == DateTimeKind.Utc
                ? TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.Local)
                : dateTime;
            return convertedDateTime.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}