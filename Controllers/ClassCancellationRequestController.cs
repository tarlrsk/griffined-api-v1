using griffined_api.Dtos.ClassCancellationRequestDto;

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

        [HttpPost("{studyClassId}"), Authorize(Roles = "student, teacher, master, allstaff")]
        public async Task<ActionResult> AddClassCancellationRequest(int studyClassId)
        {
            return Ok(await _classCancellation.AddClassCancellationRequest(studyClassId));
        }

        [HttpGet(), Authorize(Roles = "ea, master, allstaff")]
        public async Task<ActionResult> ListAllClassCancellationRequest()
        {
            return Ok(await _classCancellation.ListAllClassCancellationRequest());
        }

        [HttpGet("{requestId}"), Authorize(Roles = "ea, master, allstaff")]
        public async Task<ActionResult> GetClassCancellationRequestDetailByRequestId(int requestId)
        {
            return Ok(await _classCancellation.GetClassCancellationRequestDetailByRequestId(requestId));
        }

        [HttpPut("take/{requestId}"), Authorize(Roles = "ea, master, allstaff")]
        public async Task<ActionResult> EaTakeRequest(int requestId)
        {
            return Ok(await _classCancellation.EaTakeRequest(requestId));
        }

        [HttpPut("release/{requestId}"), Authorize(Roles = "ea, master, allstaff")]
        public async Task<ActionResult> EaReleaseRequest(int requestId)
        {
            return Ok(await _classCancellation.EaReleaseRequest(requestId));
        }

        [HttpPut("reject/{requestId}"), Authorize(Roles = "ea, master, allstaff")]
        public async Task<ActionResult> RejectRequest(int requestId, RejectedClassCancellationRequestDto rejectedRequest)
        {
            return Ok(await _classCancellation.RejectRequest(requestId, rejectedRequest.RejectedReason));
        }

        [HttpPut("schedule/{requestId}"), Authorize(Roles = "ea, master, allstaff")]
        public async Task<ActionResult> UpdateScheduleWithClassCancellationRequest(int requestId, UpdateStudyCourseRequestDto updateRequest)
        {
            return Ok(await _classCancellation.UpdateScheduleWithClassCancellationRequest(requestId, updateRequest));
        }
    }
}