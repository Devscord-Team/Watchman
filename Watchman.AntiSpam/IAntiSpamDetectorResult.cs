namespace Watchman.AntiSpam
{
    public interface IAntiSpamDetectorResult
    {
        public int Points { get; }
        public int MaxResult { get; }
        public int Percent { get; set; }
    }
}
