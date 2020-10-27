using System.Collections.Generic;

namespace Watchman.DomainModel.Help
{
    public class ArgumentInformation
    {
        public string Name { get; set; }
        public string ExpectedTypeName { get; set; }
        public IEnumerable<Description> Descriptions { get; set; }
        public string ExampleValue { get; set; }
        public bool IsOptional { get; set; }
    }
}
