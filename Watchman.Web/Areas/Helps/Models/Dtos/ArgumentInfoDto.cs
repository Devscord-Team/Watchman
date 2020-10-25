using Watchman.DomainModel.Help;

namespace Watchman.Web.Areas.Helps.Models.Dtos
{
    public class ArgumentInfoDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ExampleValues { get; set; }

        public ArgumentInfoDto(ArgumentInformation argumentInformation)
        {
            this.Name = argumentInformation.Name;
            //this.Description = argumentInformation.Description;
            //this.ExampleValues = argumentInformation.ExampleValue;
        }
    }
}
