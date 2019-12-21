using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.DomainModel.Responses;

namespace Watchman.Web.Areas.Responses.Models.Dtos
{
    public class ResponseDto
    {
        public string OnEvent { get; set; }
        public string Message { get; set; }

        public ResponseDto()
        {
        }

        public ResponseDto(Response response)
        {
            this.OnEvent = response.OnEvent;
            this.Message = response.Message;
        }
    }
}
