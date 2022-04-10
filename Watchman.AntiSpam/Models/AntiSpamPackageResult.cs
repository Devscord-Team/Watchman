using Watchman.AntiSpam.Abstraction;

namespace Watchman.AntiSpam.Models
{
    public class AntiSpamPackageResult
    {
        public AntiSpamMessagesPackage ForPackage { get; }
        public IAntiSpamDetectorResult[] DetectorsResults { get; }
        public int PointsSum { get; }

        public AntiSpamPackageResult(AntiSpamMessagesPackage forPackage, IAntiSpamDetectorResult[] detectorsResults, int pointsSum)
        {
            this.ForPackage = forPackage;
            this.DetectorsResults = detectorsResults;
            this.PointsSum = pointsSum;
        }
    }
}
