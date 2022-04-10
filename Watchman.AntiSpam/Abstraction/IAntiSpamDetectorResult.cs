namespace Watchman.AntiSpam.Abstraction
{
    public interface IAntiSpamDetectorResult
    {
        public int Points { get; }
        public int MaxResult { get; }
        public int Percent { get; set; }
    }
}
