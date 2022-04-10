namespace Watchman.AntiSpam.Abstraction
{
    public interface IAntiSpamDetector
    {
        IAntiSpamDetector WithConfiguration<T>(IAntiSpamDetectorConfiguration<T> configuration) where T : IAntiSpamDetector;
        IAntiSpamDetectorResult Detect(AntiSpamMessagesPackage package);
    }
}
