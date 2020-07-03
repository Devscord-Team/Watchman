using System;
using Watchman.DomainModel.Responses;

namespace Watchman.Web.Areas.Responses.Models.Dtos
{
    public class ResponseDto
    {
        public Guid Id { get; set; }
        public string OnEvent { get; set; }
        public string Message { get; set; }

        public ResponseDto()
        {
        }

        public ResponseDto(Response response)
        {
            this.Id = response.Id;
            this.OnEvent = response.OnEvent;
            this.Message = response.Message;
        }
    }
}
