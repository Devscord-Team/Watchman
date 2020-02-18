namespace Devscord.DiscordFramework.Commons.Extensions
{
    public static class StringExtensions
    {
        public static string CutStart(this string value, string toCut)
        {
            return string.IsNullOrEmpty(toCut) 
                ? value
                : value[toCut.Length..];
        }
    }
}
