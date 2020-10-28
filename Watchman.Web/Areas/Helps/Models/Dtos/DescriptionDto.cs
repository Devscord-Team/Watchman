using Watchman.DomainModel.Help;

namespace Watchman.Web.Areas.Helps.Models.Dtos
{
    public class DescriptionDto
    {
        public string Language { get; set; }
        public string Text { get; set; }

        public DescriptionDto(Description description)
        {
            this.Language = description.Language;
            this.Text = description.Text;
        }
    }
}
