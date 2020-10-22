using TypeGen.Core.TypeAnnotations;
using Watchman.DomainModel.Help;

namespace Watchman.Web.Areas.Helps.Models.Dtos
{
    [ExportTsClass(OutputDir = "ClientApp/src/models")]
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
