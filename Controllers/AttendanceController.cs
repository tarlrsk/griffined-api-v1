using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/v1/student-attendance")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpPut("{studyClassId}"), Authorize(Roles = "teacher, master")]
        public async Task<ActionResult> UpdateStudentAttendance(int studyClassId, List<UpdateAttendanceRequestDto> updateAttendanceRequests)
        {
            var response = await _attendanceService.UpdateStudentAttendance(studyClassId, updateAttendanceRequests);
            return Ok(response);
        }
    }
}