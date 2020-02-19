using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.DomainModel.Responses;

namespace Watchman.Web.Server.Areas.Responses.Models.Dtos
{
    public class ResponseDto
    {
        public string OnEvent { get; set; }
        public string Message { get; set; }

        public ResponseDto(Response response)
        {
            OnEvent = response.OnEvent;
            Message = response.Message;
        }
    }
}
