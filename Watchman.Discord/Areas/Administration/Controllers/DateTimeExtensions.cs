using System;
using System.Globalization;

namespace Watchman.Discord.Areas.Administration.Controllers
{
    public static class DateTimeExtensions
    {
        public static string ToLocalTimeString(this DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.Local).ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}