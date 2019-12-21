using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Watchman.Web.Areas.Responses.Models.Dtos
{
    public class GetResponsesResponse
    {
        public IEnumerable<ResponseDto> Responses { get; set; }
    }
}
