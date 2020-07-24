using Watchman.DomainModel.Help;

namespace Watchman.Web.Areas.Helps.Models.Dtos
{
    public class DescriptionDto
    {
        public string Name { get; set; }
        public string Value { get; set; } = "Example description"; //TODO change to value from database

        public DescriptionDto(Description description)
        {
            this.Name = description.Name;
            this.Value = description.Details;
        }
    }
}
