using System.Collections.Generic;

namespace Watchman.DomainModel.Help
{
    public class ArgumentInformation
    {
        public string Name { get; private set; }
        public string ExpectedTypeName { get; private set; }
        public IEnumerable<Description> Descriptions { get; private set; }
        public string ExampleValue { get; private set; }
        public bool IsOptional { get; private set; }

        public ArgumentInformation(string name, string expectedTypeName, IEnumerable<Description> descriptions, bool isOptional)
        {
            this.Name = name;
            this.ExpectedTypeName = expectedTypeName;
            this.Descriptions = descriptions;
            this.IsOptional = isOptional;
        }

        public ArgumentInformation(string name, string expectedTypeName, IEnumerable<Description> descriptions, string exampleValue, bool isOptional)
        {
            this.Name = name;
            this.ExpectedTypeName = expectedTypeName;
            this.Descriptions = descriptions;
            this.ExampleValue = exampleValue;
            this.IsOptional = isOptional;
        }

        public ArgumentInformation SetName(string name)
        {
            this.Name = name;
            return this;
        }

        public ArgumentInformation SetExpectedTypeName(string expectedTypeName)
        {
            this.ExpectedTypeName = expectedTypeName;
            return this;
        }

        public ArgumentInformation SetDescriptions(IEnumerable<Description> descriptions)
        {
            this.Descriptions = descriptions;
            return this;
        }

        public ArgumentInformation SetExampleValue(string exampleValue)
        {
            this.ExampleValue = exampleValue;
            return this;
        }

        public ArgumentInformation SetIsOptional(bool isOptional)
        {
            this.IsOptional = isOptional;
            return this;
        }
    }
}
