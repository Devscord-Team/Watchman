using System;
using System.Collections.Generic;
using System.Text;

namespace Devscord.DiscordFramework.Commons.Extensions
{
    public static class StringExtensions
    {
        public static string CutStart(this string value, string toCut)
        {
            if(string.IsNullOrEmpty(toCut))
            {
                return value;
            }
            return value.Substring(toCut.Length, value.Length - toCut.Length);
        }
    }
}
