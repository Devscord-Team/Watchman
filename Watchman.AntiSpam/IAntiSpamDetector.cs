namespace Watchman.AntiSpam
{
    public interface IAntiSpamDetector
    {
        IAntiSpamDetector WithConfiguration(IAntiSpamDetectorConfiguration configuration);
        IAntiSpamDetectorResult Detect(AntiSpamMessagesPackage package);
    }
}
