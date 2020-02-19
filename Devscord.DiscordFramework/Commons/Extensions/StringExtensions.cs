using System.Text.RegularExpressions;

namespace Devscord.DiscordFramework.Commons.Extensions
{
    public static class StringExtensions
    {
        public static string CutStart(this string value, string toCut)
        {
            return string.IsNullOrEmpty(toCut) 
                ? value
                : Regex.Replace(value, $@"^{toCut}", string.Empty, RegexOptions.Compiled);
        }
    }
}
