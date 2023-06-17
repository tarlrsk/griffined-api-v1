using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    [ApiController]
    [Route("api/v1/teacher")]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;
        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpGet, Authorize(Roles = "oa, ea")]
        public async Task<ActionResult<ServiceResponse<List<GetTeacherDto>>>> Get()
        {
            return Ok(await _teacherService.GetTeacher());
        }

        [HttpGet("{id}"), Authorize(Roles = "oa, ea")]
        public async Task<ActionResult<ServiceResponse<List<GetTeacherDto>>>> GetTeacherById(int id)
        {
            var response = await _teacherService.GetTeacherById(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpGet("by-token"), Authorize(Roles = "teacher")]
        public async Task<ActionResult<ServiceResponse<List<GetTeacherDto>>>> GetTeacherByToken()
        {
            var response = await _teacherService.GetTeacherByToken();
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPost, Authorize(Roles = "oa, ea")]
        public async Task<ActionResult<ServiceResponse<List<GetTeacherDto>>>> AddTeacher(AddTeacherDto newTeacher)
        {
            return Ok(await _teacherService.AddTeacher(newTeacher));
        }

        [HttpPut, Authorize(Roles = "oa, ea")]
        public async Task<ActionResult<ServiceResponse<List<GetTeacherDto>>>> UpdateTeacher(UpdateTeacherDto updatedTeacher)
        {
            var response = await _teacherService.UpdateTeacher(updatedTeacher);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPut("student/attendance"), Authorize(Roles = "teacher")]
        public async Task<ActionResult<ServiceResponse<GetStudentPrivateClassDto>>> UpdateStudentAttendance(UpdateStudentPrivateClassDto updatedStudentAttendance)
        {
            var response = await _teacherService.UpdateStudentAttendance(updatedStudentAttendance);
            if (response.Data is null)
                return NotFound(response);
            return response;
        }

        [HttpDelete("{id}"), Authorize(Roles = "oa, ea")]
        public async Task<ActionResult<ServiceResponse<GetTeacherDto>>> DeleteTeacher(int id)
        {
            var response = await _teacherService.DeleteTeacher(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPut("activate/{id}"), Authorize(Roles = "oa, ea")]
        public async Task<ActionResult<ServiceResponse<GetStudentDto>>> EnableStudent(int id)
        {
            var response = await _teacherService.EnableTeacher(id);
            if (response.Success != true)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPut("deactivate/{id}"), Authorize(Roles = "oa, ea")]
        public async Task<ActionResult<ServiceResponse<GetStudentDto>>> DisableStudent(int id)
        {
            var response = await _teacherService.DisableTeacher(id);
            if (response.Success != true)
                return NotFound(response);
            return Ok(response);
        }
    }
}