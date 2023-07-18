using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


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

        [HttpGet, Authorize(Roles = "ep, ea, oa, master")]
        public async Task<ActionResult> GetStudent()
        {
            return Ok(await _studentService.GetStudent());
        }

        [HttpGet("{studentId}"), Authorize(Roles = "ep, ea, oa, teacher, master")]
        public async Task<ActionResult> GetStudentByStudentId(string studentId)
        {
            var response = await _studentService.GetStudentByStudentId(studentId);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }
        [HttpGet("by-token"), Authorize(Roles = "student, master")]
        public async Task<ActionResult> GetStudentByToken()
        {
            var response = await _studentService.GetStudentByToken();
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPost, Authorize(Roles = "ep, master")]
        public async Task<ActionResult> AddStudent([FromForm] AddStudentRequestDto newStudent, IFormFile profilePicture, ICollection<IFormFile> files)
        {
            return Ok(await _studentService.AddStudent(newStudent, profilePicture, files));
        }

        [HttpPut, Authorize(Roles = "ep, ea, oa, master")]
        public async Task<ActionResult> UpdateStudent([FromForm] UpdateStudentRequestDto updatedStudent, IFormFile profilePicture, ICollection<IFormFile> files)
        {
            var response = await _studentService.UpdateStudent(updatedStudent, profilePicture, files);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpDelete("{id}"), Authorize(Roles = "ep, ea, oa, master")]
        public async Task<ActionResult> DeleteStudent(int id)
        {
            var response = await _studentService.DeleteStudent(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }


        [HttpPut("activate/{id}"), Authorize(Roles = "ep, ea, oa, master")]
        public async Task<ActionResult> EnableStudent(int id)
        {
            var response = await _studentService.EnableStudent(id);
            if (response.Success != true)
                return NotFound(response);
            return Ok(response);
        }


        [HttpPut("deactivate/{id}"), Authorize(Roles = "ep, ea, oa, master")]
        public async Task<ActionResult> DisableStudent(int id)
        {
            var response = await _studentService.DisableStudent(id);
            if (response.Success != true)
                return NotFound(response);
            return Ok(response);
        }
    }
}