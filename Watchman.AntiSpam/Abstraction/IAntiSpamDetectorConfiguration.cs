namespace Watchman.AntiSpam.Abstraction
{
    public interface IAntiSpamDetectorConfiguration<T> where T : IAntiSpamDetector
    {
        public int MaxResult { get; }
    }
}
