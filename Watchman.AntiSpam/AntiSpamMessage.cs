namespace Watchman.AntiSpam
{
    public struct AntiSpamMessage
    {
        public ulong ChannelId;
        public ulong UserId;
        public DateTime DateTime;
        public string Message;

        public AntiSpamMessage(ulong channelId, ulong userId, DateTime dateTime, string message)
        {
            this.ChannelId = channelId;
            this.UserId = userId;
            this.DateTime = dateTime;
            this.Message = message;
        }
    }
}
