using System.Collections.Generic;
using System.Linq;
using Watchman.DomainModel.Help;

namespace Watchman.Web.Areas.Helps.Models.Dtos
{
    public class HelpInformationDto
    {
        public string CommandName { get; set; }
        public string AreaName { get; set; }
        public IEnumerable<ArgumentInfoDto> Arguments { get; set; }
        public IEnumerable<DescriptionDto> Descriptions { get; set; }
        public ulong ServerId { get; set; }
        public string DefaultLanguage { get; set; }
        public string ExampleUsage { get; set; }
        public bool IsDefault { get; set; }

        public HelpInformationDto(HelpInformation helpInformation)
        {
            this.CommandName = helpInformation.CommandName;
            this.AreaName = helpInformation.AreaName;
            this.Arguments = helpInformation.ArgumentInformations.Select(x => new ArgumentInfoDto(x));
            this.Descriptions = helpInformation.Descriptions.Select(x => new DescriptionDto(x));
            this.ServerId = helpInformation.ServerId;
            this.DefaultLanguage = helpInformation.DefaultLanguage;
            this.ExampleUsage = helpInformation.ExampleUsage;
            this.IsDefault = helpInformation.IsDefault;
        }
    }
}
