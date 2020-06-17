using Watchman.DomainModel.Help;

namespace Watchman.Web.Areas.Helps.Models.Dtos
{
    public class DescriptionDto
    {
        public string Name { get; set; }
        public string Value { get; set; } = "Example description"; //TODO change to value from database

        public DescriptionDto(Description description)
        {
            Name = description.Name;
            Value = description.Details;
        }
    }
}
