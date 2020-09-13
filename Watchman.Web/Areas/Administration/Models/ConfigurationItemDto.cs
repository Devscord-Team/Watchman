using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Watchman.DomainModel.Settings;

namespace Watchman.Web.Areas.Administration.Models
{
    public class ConfigurationItemDto
    {
        public ulong ServerId { get; set; }
        public object Value { get; set; }
        public string Name { get; set; }

        public ConfigurationItemDto(ConfigurationItem configurationItem)
        {
            Name = configurationItem.Name;
            Value = configurationItem.Value;
            ServerId = configurationItem.ServerId;
        }
    }
}