using Watchman.DomainModel.Help;

namespace Watchman.Web.Server.Areas.Helps.Models.Dtos
{
    public class ArgumentInfoDto
    {
        public string Name { get; set; }
        public string Description { get; set; } = "Example description"; //TODO change to value from database

        public ArgumentInfoDto(ArgumentInfo argumentInfo)
        {
            Name = argumentInfo.Name;
        }
    }
}
