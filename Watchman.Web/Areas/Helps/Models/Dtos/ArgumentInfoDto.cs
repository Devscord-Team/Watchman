using System.Collections.Generic;
using System.Linq;
using Watchman.DomainModel.Help;

namespace Watchman.Web.Areas.Helps.Models.Dtos
{
    public class ArgumentInfoDto
    {
        public string Name { get; set; }
        public string ExpectedTypeName { get; set; }
        public IEnumerable<DescriptionDto> Descriptions { get; set; }
        public string ExampleValue { get; set; }
        public bool IsOptional { get; set; }

        public ArgumentInfoDto(ArgumentInformation argumentInformation)
        {
            this.Name = argumentInformation.Name;
            this.ExpectedTypeName = argumentInformation.ExpectedTypeName;
            this.Descriptions = argumentInformation.Descriptions.Select(x => new DescriptionDto(x));
            this.ExampleValue = argumentInformation.ExampleValue;
            this.IsOptional = argumentInformation.IsOptional;
        }
    }
}
