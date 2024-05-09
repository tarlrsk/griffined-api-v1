using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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

        [HttpGet("{studyClassId}"), Authorize(Roles = "teacher, master, allstaff")]
        public async Task<ActionResult> GetClassAttendance(int studyClassId)
        {
            var response = await _attendanceService.GetClassAttendance(studyClassId);
            if (response == null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPut("{studyClassId}"), Authorize(Roles = "teacher, master, allstaff")]
        public async Task<ActionResult> UpdateStudentAttendance(int studyClassId, List<UpdateAttendanceRequestDto> updateAttendanceRequests)
        {
            var response = await _attendanceService.UpdateStudentAttendance(studyClassId, updateAttendanceRequests);
            return Ok(response);
        }
    }
}