using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.AntiSpam.Abstraction;

namespace Watchman.AntiSpam.Detectors
{
    public class LinksDetector : IAntiSpamDetector
    {
        public IAntiSpamDetectorResult Detect(AntiSpamMessagesPackage package)
        {
            throw new NotImplementedException();
        }

        public IAntiSpamDetector WithConfiguration<T>(IAntiSpamDetectorConfiguration<T> configuration) where T : IAntiSpamDetector
        {
            throw new NotImplementedException();
        }
    }
}
