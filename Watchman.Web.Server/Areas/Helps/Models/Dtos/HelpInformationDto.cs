using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.DomainModel.Help;

namespace Watchman.Web.Server.Areas.Helps.Models.Dtos
{
    public class HelpInformationDto
    {
        public string Name { get; set; }
        public string MethodFullName { get; set; }
        public IEnumerable<ArgumentInfoDto> Arguments { get; set; }
        public IEnumerable<DescriptionDto> Descriptions { get; set; }
        public bool IsDefault { get; set; }

        public HelpInformationDto(HelpInformation helpInformation)
        {
            Name = helpInformation.Names.First();
            MethodFullName = helpInformation.MethodFullName;
            Arguments = helpInformation.ArgumentInfos.Select(x => new ArgumentInfoDto(x));
            Descriptions = helpInformation.Descriptions.Select(x => new DescriptionDto(x));
            IsDefault = helpInformation.IsDefault;
        }
    }
}
