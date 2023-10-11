using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/v1/available")]

    [Authorize(Roles = "ea, master")]

    public class CheckAvailable : ControllerBase
    {
        private readonly ICheckAvailableService _checkAvailable;
        public CheckAvailable(ICheckAvailableService checkAvailable)
        {
            _checkAvailable = checkAvailable;

        }

        [HttpPost("schedule"), Authorize(Roles = "master, ea")]
        public async Task<ActionResult> GetAvailableSchedule(RequestedScheduleRequestDto requestedSchedule)
        {
            return Ok(await _checkAvailable.GetAvailableSchedule(requestedSchedule));
        }

        [HttpPost("teacher"), Authorize(Roles = "master, ea")]
        public async Task<ActionResult> GetAvailableTeacherForAppointment(int? appointmentId, List<LocalAppointmentRequestDto> appointmentRequestDtos)
        {
            return Ok(await _checkAvailable.GetAvailableTeacherForAppointment(appointmentId, appointmentRequestDtos));
        }

        [HttpPost("appointment"), Authorize(Roles = "master, ea")]
        public async Task<ActionResult> CheckAppointmentConflict(CheckAppointmentConflictRequestDto requestDto)
        {
            return Ok(await _checkAvailable.CheckAppointmentConflict(requestDto));
        }

        [HttpPost("student-adding"), Authorize(Roles = "master, ea")]
        public async Task<ActionResult> CheckStudentAddingConflict(StudentAddingConflictRequestDto requestDto)
        {
            return Ok(await _checkAvailable.CheckStudentAddingConflict(requestDto));
        }

    }
}