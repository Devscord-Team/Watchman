using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.AntiSpam.Abstraction;
using Watchman.AntiSpam.Models;

namespace Watchman.AntiSpam
{
    /// <summary>
    /// There should be one AntiSpamFlow per discord server. 
    /// Every AntiSpamFlow has own list of detectors and each has own configuration.
    /// Discord servers should have possility to configure which detectors want to use and provide detailed configuration to each of them.
    /// </summary>
    public class AntiSpamFlow
    {
        private IAntiSpamDetector[] detectors;
        private List<IAntiSpamDetector> tempDetectors;

        public AntiSpamFlow AddDetector<T, C>(C configuration)
            where T : IAntiSpamDetector, new()
            where C : IAntiSpamDetectorConfiguration<T>
        {
            this.tempDetectors.Add(new T().WithConfiguration(configuration));
            return this;
        }

        public AntiSpamFlow Build()
        {
            this.detectors = this.tempDetectors.ToArray();
            this.tempDetectors = new();
            return this;
        }

        public AntiSpamPackageResult TryDetect(AntiSpamMessagesPackage package)
        {
            var detectorsResults = this.detectors
                .Select(x => x.Detect(package))
                .ToArray();
            return new AntiSpamPackageResult(package, detectorsResults, detectorsResults.Sum(x => x.Points));
        }
    }
}
