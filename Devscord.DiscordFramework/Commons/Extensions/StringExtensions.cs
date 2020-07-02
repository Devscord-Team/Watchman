namespace Devscord.DiscordFramework.Commons.Extensions
{
    public static class StringExtensions
    {
        public static string CutStart(this string value, string toCut)
        {
            return string.IsNullOrEmpty(toCut)
                ? value
                : Replace(value, toCut);
        }

        private static string Replace(string value, string toCut)
        {
            if (!value.StartsWith(toCut))
            {
                return value;
            }
            return value[toCut.Length..];
        }
    }
}
