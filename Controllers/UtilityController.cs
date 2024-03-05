using griffined_api.Services.ClientFirebaseService;

namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/v1/utilities")]
    public class FirebaseController : ControllerBase
    {
        private readonly IClientFirebaseService _clientFirebaseService;

        public FirebaseController(IClientFirebaseService clientFirebaseService)
        {
            _clientFirebaseService = clientFirebaseService;
        }

        [HttpPut("firebase/students/{studentId}")]
        public async Task<IActionResult> AddStudentFirebaseId(int studentId)
        {
            await _clientFirebaseService.AddStudentFirebaseId(studentId);

            return Ok();
        }

        [HttpPut("studentcode/students")]
        public async Task<IActionResult> AddStudentCode()
        {
            await _clientFirebaseService.AddStudentCode();

            return Ok();
        }

        [HttpPut("firebase/teachers/{teacherId}")]
        public async Task<IActionResult> AddTeacherFirebaseId(int teacherId)
        {
            await _clientFirebaseService.AddTeacherFirebaseId(teacherId);

            return Ok();
        }
    }
}