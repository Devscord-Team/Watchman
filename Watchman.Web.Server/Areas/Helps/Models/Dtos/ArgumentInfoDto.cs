using Watchman.DomainModel.Help;

namespace Watchman.Web.Server.Areas.Helps.Models.Dtos
{
    public class ArgumentInfoDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ExampleValues { get; set; }

        public ArgumentInfoDto(ArgumentInfo argumentInfo)
        {
            Name = argumentInfo.Name;
            Description = argumentInfo.Description;
            ExampleValues = argumentInfo.ExampleValues;
        }
    }
}
