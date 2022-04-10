using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.AntiSpam.Abstraction;

namespace Watchman.AntiSpam.Detectors.Configurations
{
    public class LinksDetectorConfiguration : IAntiSpamDetectorConfiguration<LinksDetector>
    {
        public int MaxResult { get; set; }
    }
}
