namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/v1/student")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet, Authorize(Roles = "ec, ea, oa, master, allstaff")]
        public async Task<ActionResult> GetStudent()
        {
            return Ok(await _studentService.GetStudent());
        }

        [HttpGet("{studentId}"), Authorize(Roles = "ec, ea, oa, teacher, master, allstaff")]
        public async Task<ActionResult> GetStudentByStudentId(string studentId)
        {
            var response = await _studentService.GetStudentByStudentId(studentId);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }
        [HttpGet("by-token"), Authorize(Roles = "student, master, allstaff")]
        public async Task<ActionResult> GetStudentByToken()
        {
            var response = await _studentService.GetStudentByToken();
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPost, Authorize(Roles = "ec, master, allstaff")]
        public async Task<ActionResult> AddStudent([FromForm] AddStudentRequestDto newStudent, IFormFile? newProfilePicture, List<IFormFile>? filesToUpload)
        {
            return Ok(await _studentService.AddStudent(newStudent, newProfilePicture, filesToUpload));
        }

        [HttpPut, Authorize(Roles = "ec, ea, oa, master, allstaff")]
        public async Task<ActionResult> UpdateStudent([FromForm] UpdateStudentRequestDto updatedStudent, IFormFile? updatedProfilePicture, List<IFormFile>? filesToUpload)
        {
            var response = await _studentService.UpdateStudent(updatedStudent, updatedProfilePicture, filesToUpload);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpDelete("{id}"), Authorize(Roles = "ec, ea, oa, master, allstaff")]
        public async Task<ActionResult> DeleteStudent(int id)
        {
            var response = await _studentService.DeleteStudent(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }


        [HttpPut("activate/{id}"), Authorize(Roles = "ec, ea, oa, master, allstaff")]
        public async Task<ActionResult> EnableStudent(int id)
        {
            var response = await _studentService.EnableStudent(id);
            if (response.Success != true)
                return NotFound(response);
            return Ok(response);
        }


        [HttpPut("deactivate/{id}"), Authorize(Roles = "ec, ea, oa, master, allstaff")]
        public async Task<ActionResult> DisableStudent(int id)
        {
            var response = await _studentService.DisableStudent(id);
            if (response.Success != true)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPut("change-password/{uid}"), Authorize(Roles = "ec, ea, oa, master, allstaff")]
        public async Task<ActionResult> ChangePasswordWithFirebaseUid(string uid, ChangeUserPasswordDto password)
        {
            return Ok(await _studentService.ChangePasswordWithFirebaseId(uid, password));
        }
    }
}