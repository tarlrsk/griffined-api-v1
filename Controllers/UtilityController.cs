using System.Net;
using griffined_api.Services.UtilityService;

namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/v1/utilities")]
    public class UtilityController : ControllerBase
    {
        private readonly IUtilityService _utilityService;

        public UtilityController(IUtilityService utilityService)
        {
            _utilityService = utilityService;
        }

        [HttpPut("firebase/student")]
        public async Task<IActionResult> AddStudentFirebaseId()
        {
            await _utilityService.AddStudentFirebaseId();

            return Ok();
        }

        [HttpPut("studentcode/students")]
        public async Task<IActionResult> AddStudentCode()
        {
            await _utilityService.AddStudentCode();

            return Ok();
        }

        [HttpPut("firebase/teacher")]
        public async Task<IActionResult> AddTeacherFirebaseId()
        {
            await _utilityService.AddTeacherFirebaseId();

            return Ok();
        }

        [HttpPut("firebase/staff")]
        public async Task<IActionResult> AddStaffFirebaseId()
        {
            await _utilityService.AddStaffFirebaseId();

            return Ok();
        }

        [HttpDelete("firebase/auth")]
        public async Task<IActionResult> DeleteFirebaseAuthentication()
        {
            await _utilityService.DeleteFirebaseAuthentication();

            return Ok();
        }

        [HttpPut("update/study-classes/number")]
        public IActionResult UpdateStudyClassNumberByStudySubjectId()
        {
            _utilityService.UpdateStudyClassNumber();

            return Ok(ResponseWrapper.Success(HttpStatusCode.OK));
        }

        [HttpPut("update/study-course/status")]
        public async Task<IActionResult> UpdateStudyCourseStatus()
        {
            await _utilityService.UpdateStudyCourseStatus();

            return Ok(ResponseWrapper.Success(HttpStatusCode.OK));
        }

        [HttpPut("update/student/expiry-date")]
        public async Task<IActionResult> UpdateStudentExpiryDate()
        {
            await _utilityService.UpdateStudentExpiryDate();

            return Ok(ResponseWrapper.Success(HttpStatusCode.OK));
        }
    }
}