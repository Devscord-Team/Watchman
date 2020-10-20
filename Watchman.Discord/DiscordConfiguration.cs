namespace Watchman.Discord
{
    public class DiscordConfiguration
    {
        public string Token { get; set; }
        public string MongoDbConnectionString { get; set; }
        public bool SendOnlyUnknownExceptionInfo { get; set; }
<<<<<<< HEAD
        public ulong ExceptionChannelID { get; set; }
        public string LiteDbConnectionString { get; set; }
=======
        public ulong ExceptionChannelId { get; set; }
        public ulong ExceptionServerId { get; set; }
>>>>>>> master
    }
}
