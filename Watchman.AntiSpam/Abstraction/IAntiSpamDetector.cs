namespace Watchman.AntiSpam.Abstraction
{
    public interface IAntiSpamDetector
    {
        IAntiSpamDetector WithConfiguration(IAntiSpamDetectorConfiguration configuration);
        IAntiSpamDetectorResult Detect(AntiSpamMessagesPackage package);
    }
}
