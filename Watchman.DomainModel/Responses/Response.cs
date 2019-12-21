using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Responses
{
    public class Response : Entity
    {
        public string OnEvent { get; set; }
        public string Message { get; set; }
    }
}
