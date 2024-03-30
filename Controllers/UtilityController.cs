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

        [HttpPut("firebase/students/{studentId}")]
        public async Task<IActionResult> AddStudentFirebaseId(int studentId)
        {
            await _utilityService.AddStudentFirebaseId(studentId);

            return Ok();
        }

        [HttpPut("studentcode/students")]
        public async Task<IActionResult> AddStudentCode()
        {
            await _utilityService.AddStudentCode();

            return Ok();
        }

        [HttpPut("firebase/teachers/{teacherId}")]
        public async Task<IActionResult> AddTeacherFirebaseId(int teacherId)
        {
            await _utilityService.AddTeacherFirebaseId(teacherId);

            return Ok();
        }

        [HttpDelete("firebase/auth")]
        public async Task<IActionResult> DeleteFirebaseAuthentication()
        {
            await _utilityService.DeleteFirebaseAuthentication();

            return Ok();
        }
    }
}