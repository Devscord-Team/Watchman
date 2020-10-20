using Watchman.DomainModel.Configuration;

namespace Watchman.Web.Areas.Administration.Models
{
    public class ConfigurationItemDto
    {
        public ulong ServerId { get; set; }
        public object Value { get; set; }
        public string Name { get; set; }

        public ConfigurationItemDto(ConfigurationItem configurationItem)
        {
            this.Name = configurationItem.Name;
            this.Value = configurationItem.Value;
            this.ServerId = configurationItem.ServerId;
        }
    }
}