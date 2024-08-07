namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/v1/available")]

    public class CheckAvailable : ControllerBase
    {
        private readonly ICheckAvailableService _checkAvailable;
        public CheckAvailable(ICheckAvailableService checkAvailable)
        {
            _checkAvailable = checkAvailable;

        }

        [HttpPost("schedule"), Authorize(Roles = "master, allstaff, ea")]
        public async Task<ActionResult> GetAvailableSchedule(RequestedScheduleRequestDto requestedSchedule)
        {
            return Ok(await _checkAvailable.GetAvailableSchedule(requestedSchedule));
        }

        [HttpPost("teacher"), Authorize(Roles = "master, allstaff, ea")]
        public async Task<ActionResult> GetAvailableTeacherForAppointment(int? appointmentId, List<LocalAppointmentRequestDto> appointmentRequestDtos)
        {
            return Ok(await _checkAvailable.GetAvailableTeacherForAppointment(appointmentId, appointmentRequestDtos));
        }

        [HttpPost("appointment"), Authorize(Roles = "master, allstaff, ea")]
        public async Task<ActionResult> CheckAppointmentConflict(CheckAppointmentConflictRequestDto requestDto)
        {
            return Ok(await _checkAvailable.CheckAppointmentConflict(requestDto));
        }

        [HttpPost("student-adding"), Authorize(Roles = "master, allstaff, ec, ea")]
        public async Task<ActionResult> CheckStudentAddingConflict(StudentAddingConflictRequestDto requestDto)
        {
            return Ok(await _checkAvailable.CheckStudentAddingConflict(requestDto));
        }

    }
}