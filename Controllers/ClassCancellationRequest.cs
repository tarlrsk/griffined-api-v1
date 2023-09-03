using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Services.ClassCancellationRequest;

namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/v1/class-cancellation-request")]

    public class ClassCancellationRequest : ControllerBase
    {
        private readonly IClassCancellationRequest _classCancellation;

        public ClassCancellationRequest(IClassCancellationRequest classCancellation)
        {
            _classCancellation = classCancellation;
        }
        
        [HttpPost]
        public async Task<ActionResult> AddClassCancellationRequest()
        {
            return Ok();
        }

    }
}