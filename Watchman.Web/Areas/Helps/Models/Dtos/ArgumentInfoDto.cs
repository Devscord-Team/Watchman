using Watchman.DomainModel.Help;

namespace Watchman.Web.Areas.Helps.Models.Dtos
{
    public class ArgumentInfoDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ExampleValues { get; set; }

        public ArgumentInfoDto(ArgumentInfo argumentInfo)
        {
            this.Name = argumentInfo.Name;
            this.Description = argumentInfo.Description;
            this.ExampleValues = argumentInfo.ExampleValues;
        }
    }
}
