using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    [ApiController]
    [Route("api/teacher")]
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

        [HttpGet("me"), Authorize(Roles = "teacher")]
        public async Task<ActionResult<ServiceResponse<List<GetTeacherDto>>>> GetTeacherByMe()
        {
            var response = await _teacherService.GetTeacherByMe();
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpGet("course-count"), Authorize(Roles = "oa, ea, teacher")]
        public async Task<ActionResult<ServiceResponse<List<GetTeacherWithCourseCountDto>>>> GetTeacherWithCourseCount()
        {
            return Ok(await _teacherService.GetTeacherWithCourseCount());
        }

        [HttpGet("course/{teacherId}"), Authorize(Roles = "oa, ea")]
        public async Task<ActionResult<ServiceResponse<List<GetTeacherCourseWithClassesDto>>>> GetTeacherCourseWithClassesByTeacherId(int teacherId)
        {
            var response = await _teacherService.GetTeacherCourseWithClassesByTeacherId(teacherId);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }
        [HttpGet("course/me"), Authorize(Roles = "teacher")]
        public async Task<ActionResult<ServiceResponse<List<GetTeacherCourseWithClassesDto>>>> GetTeacherCourseWithClassesByMe()
        {
            var response = await _teacherService.GetTeacherCourseWithClassesByMe();
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpGet("student/attendance/{classId}"), Authorize(Roles = "oa, ea, teacher")]
        public async Task<ActionResult<ServiceResponse<List<GetStudentAttendanceDto>>>> GetStudentAttendanceByClassId(int classId)
        {
            var response = await _teacherService.GetStudentAttendanceByClassId(classId);
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

        [HttpPut("class/status"), Authorize(Roles = "teacher")]
        public async Task<ActionResult<ServiceResponse<GetTeacherPrivateClassDto>>> UpdateClassStatus(UpdateTeacherPrivateClassDto updatedClassStatus)
        {
            var response = await _teacherService.UpdateClassStatus(updatedClassStatus);
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

        // [HttpGet("LeavingRequest/Get")]
        // public async Task<ActionResult<ServiceResponse<List<GetTeacherLeavingRequestDto>>>> GetTeacherLeavingRequest()
        // {
        //     return Ok(await _teacherService.GetTeacherLeavingRequest());
        // }

        // [HttpGet("LeavingRequest/Get/{id}")]
        // public async Task<ActionResult<ServiceResponse<List<GetTeacherLeavingRequestDto>>>> GetTeacherLeavingRequestById(int id)
        // {
        //     var response = await _teacherService.GetTeacherLeavingRequestById(id);
        //     if (response.Data is null)
        //         return NotFound(response);
        //     return Ok(response);
        // }

        // [HttpGet("LeavingRequest/Get/{teacherId}")]
        // public async Task<ActionResult<ServiceResponse<GetTeacherLeavingRequestDto>>> GetTeacherLeavingRequestByTeacherId(int teacherId)
        // {
        //     var response = await _teacherService.GetTeacherLeavingRequestByTeacherId(teacherId);
        //     if (response.Data is null)
        //         return NotFound(response);
        //     return Ok(response);
        // }

        // [HttpPost("LeavingRequest/Post")]
        // public async Task<ActionResult<ServiceResponse<List<GetTeacherLeavingRequestDto>>>> AddTeacherLeavingRequest(AddTeacherLeavingRequestDto newRequest)
        // {
        //     return Ok(await _teacherService.AddTeacherLeavingRequest(newRequest));
        // }

        // [HttpPut("LeavingRequest/Put")]
        // public async Task<ActionResult<ServiceResponse<GetTeacherLeavingRequestDto>>> UpdateTeacherLeavingRequest(UpdateTeacherLeavingRequestDto updatedRequest)
        // {
        //     var response = await _teacherService.UpdateTeacherLeavingRequest(updatedRequest);
        //     if (response.Data is null)
        //         return NotFound(response);
        //     return Ok(response);
        // }

        // [HttpDelete("LeavingRequest/Delete/{id}")]
        // public async Task<ActionResult<ServiceResponse<List<GetTeacherLeavingRequestDto>>>> DeleteTeacherLeavingRequest(int id)
        // {
        //     var response = await _teacherService.DeleteTeacherLeavingRequest(id);
        //     if (response.Data is null)
        //         return NotFound(response);
        //     return Ok(response);
        // }
    }
}