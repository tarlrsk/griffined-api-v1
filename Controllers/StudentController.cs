using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet("Get"), Authorize(Roles = "ep, ea, oa")]
        public async Task<ActionResult<ServiceResponse<List<GetStudentDto>>>> GetStudent()
        {
            return Ok(await _studentService.GetStudent());
        }

        [HttpGet("Get/{id}"), Authorize(Roles = "ep, ea, oa, teacher")]
        public async Task<ActionResult<ServiceResponse<List<GetStudentDto>>>> GetStudentById(int id)
        {
            var response = await _studentService.GetStudentById(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }
        [HttpGet("Get/Me"), Authorize(Roles = "student")]
        public async Task<ActionResult<ServiceResponse<List<GetStudentDto>>>> GetStudentByMe()
        {
            var response = await _studentService.GetStudentByMe();
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpGet("CourseCount/Get"), Authorize(Roles = "ep, ea, oa, teacher")]
        public async Task<ActionResult<ServiceResponse<List<GetStudentWithCourseRegisteredCountDto>>>> GetStudentWithCourseRegistered()
        {
            return Ok(await _studentService.GetStudentWithCoursesRegistered());
        }

        [HttpGet("Course/Get/{studentId}"), Authorize(Roles = "ep, ea, oa, teacher")]
        public async Task<ActionResult<ServiceResponse<List<GetStudentCourseWithClassesDto>>>> GetStudentCourseWithClassesByStudentId(int studentId)
        {
            var response = await _studentService.GetStudentCourseWithClassesByStudentId(studentId);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }
        [HttpGet("Course/Get/Me"), Authorize(Roles = "student")]
        public async Task<ActionResult<ServiceResponse<List<GetStudentCourseWithClassesDto>>>> GetStudentCourseWithClassesByMe()
        {
            var response = await _studentService.GetStudentCourseWithClassesByMe();
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPost("Post"), Authorize(Roles = "ep")]
        public async Task<ActionResult<ServiceResponse<List<GetStudentDto>>>> AddStudent(AddStudentDto newStudent)
        {
            return Ok(await _studentService.AddStudent(newStudent));
        }

        [HttpPut("Put"), Authorize(Roles = "ep, ea, oa")]
        public async Task<ActionResult<ServiceResponse<List<GetStudentDto>>>> UpdateStudent(UpdateStudentDto updatedStudent)
        {
            var response = await _studentService.UpdateStudent(updatedStudent);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpDelete("Delete/{id}"), Authorize(Roles = "ep, ea, oa")]
        public async Task<ActionResult<ServiceResponse<GetStudentDto>>> DeleteStudent(int id)
        {
            var response = await _studentService.DeleteStudent(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }


        [HttpGet("Enable/{id}"), Authorize(Roles = "ep, ea, oa")]
        public async Task<ActionResult<ServiceResponse<GetStudentDto>>> EnableStudent(int id)
        {
            var response = await _studentService.EnableStudent(id);
            if (response.Success != true)
                return NotFound(response);
            return Ok(response);
        }


        [HttpDelete("Disable/{id}"), Authorize(Roles = "ep, ea, oa")]
        public async Task<ActionResult<ServiceResponse<GetStudentDto>>> DisableStudent(int id)
        {
            var response = await _studentService.DisableStudent(id);
            if (response.Success != true)
                return NotFound(response);
            return Ok(response);
        }
    }
}