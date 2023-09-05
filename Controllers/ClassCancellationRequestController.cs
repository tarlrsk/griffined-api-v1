using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/v1/class-cancellation")]

    public class ClassCancellationRequestController : ControllerBase
    {
        private readonly IClassCancellationRequestService _classCancellation;

        public ClassCancellationRequestController(IClassCancellationRequestService classCancellation)
        {
            _classCancellation = classCancellation;
        }
        
        [HttpPost("{studyClassId}"), Authorize(Roles = "student, teacher, master")]
        public async Task<ActionResult> AddClassCancellationRequest(int studyClassId)
        {
            return Ok(await _classCancellation.AddClassCancellationRequest(studyClassId));
        }

    }
}