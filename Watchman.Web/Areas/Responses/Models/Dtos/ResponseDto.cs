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
            Id = response.Id;
            OnEvent = response.OnEvent;
            Message = response.Message;
        }
    }
}
