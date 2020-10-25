using System.Collections.Generic;
using System.Linq;
using Watchman.DomainModel.Help;

namespace Watchman.Web.Areas.Helps.Models.Dtos
{
    public class HelpInformationDto
    {
        public string Name { get; set; }
        public IEnumerable<ArgumentInfoDto> Arguments { get; set; }
        public IEnumerable<DescriptionDto> Descriptions { get; set; }
        public bool IsDefault { get; set; }

        public HelpInformationDto(HelpInformation helpInformation)
        {
            this.Name = helpInformation.CommandName;
            this.Arguments = helpInformation.ArgumentInformations.Select(x => new ArgumentInfoDto(x));
            this.Descriptions = helpInformation.Descriptions.Select(x => new DescriptionDto(x));
            this.IsDefault = helpInformation.IsDefault;
        }
    }
}
