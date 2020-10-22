using System.Collections.Generic;
using System.Linq;
using TypeGen.Core.TypeAnnotations;
using Watchman.DomainModel.Help;

namespace Watchman.Web.Areas.Helps.Models.Dtos
{
    [ExportTsClass(OutputDir = "ClientApp/src/models")]
    public class HelpInformationDto
    {
        public string Name { get; set; }
        public string MethodFullName { get; set; }
        public IEnumerable<ArgumentInfoDto> Arguments { get; set; }
        public IEnumerable<DescriptionDto> Descriptions { get; set; }
        public bool IsDefault { get; set; }

        public HelpInformationDto(HelpInformation helpInformation)
        {
            this.Name = helpInformation.Names.First();
            this.MethodFullName = helpInformation.MethodFullName;
            this.Arguments = helpInformation.ArgumentInfos.Select(x => new ArgumentInfoDto(x));
            this.Descriptions = helpInformation.Descriptions.Select(x => new DescriptionDto(x));
            this.IsDefault = helpInformation.IsDefault;
        }
    }
}
