using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    [ApiController]
    [Route("api/v1/teachers")]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;
        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        // [HttpGet, Authorize(Roles = "oa, ea, ec, master")]
        [HttpGet, AllowAnonymous]
        public async Task<ActionResult> Get()
        {
            return Ok(await _teacherService.GetTeacher());
        }

        // [HttpGet("{id}"), Authorize(Roles = "oa, ea, master")]
        [HttpGet("{id}"), AllowAnonymous]
        public async Task<ActionResult> GetTeacherById(int id)
        {
            var response = await _teacherService.GetTeacherById(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        // [HttpGet("by-token"), Authorize(Roles = "teacher, master")]
        [HttpGet("by-token"), AllowAnonymous]
        public async Task<ActionResult> GetTeacherByToken()
        {
            var response = await _teacherService.GetTeacherByToken();
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        // [HttpPost, Authorize(Roles = "oa, master")]
        [HttpPost, AllowAnonymous]
        public async Task<ActionResult> AddTeacher(AddTeacherDto newTeacher)
        {
            return Ok(await _teacherService.AddTeacher(newTeacher));
        }

        // [HttpPut, Authorize(Roles = "oa, ea, master")]
        [HttpPut, AllowAnonymous]
        public async Task<ActionResult> UpdateTeacher(UpdateTeacherDto updatedTeacher)
        {
            var response = await _teacherService.UpdateTeacher(updatedTeacher);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        // [HttpDelete("{id}"), Authorize(Roles = "oa, ea, master")]
        [HttpDelete("{id}"), AllowAnonymous]
        public async Task<ActionResult> DeleteTeacher(int id)
        {
            var response = await _teacherService.DeleteTeacher(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        // [HttpPut("activate/{id}"), Authorize(Roles = "oa, ea, master")]
        [HttpPut("activate/{id}"), AllowAnonymous]
        public async Task<ActionResult<ServiceResponse<StudentResponseDto>>> EnableStudent(int id)
        {
            var response = await _teacherService.EnableTeacher(id);
            if (response.Success != true)
                return NotFound(response);
            return Ok(response);
        }

        // [HttpPut("deactivate/{id}"), Authorize(Roles = "oa, ea, master")]
        [HttpPut("deactivate/{id}"), AllowAnonymous]
        public async Task<ActionResult> DisableStudent(int id)
        {
            var response = await _teacherService.DisableTeacher(id);
            if (response.Success != true)
                return NotFound(response);
            return Ok(response);
        }

        // [HttpPut("change-password/{uid}"), Authorize(Roles = "oa, master")]
        [HttpPut("change-password/{uid}"), AllowAnonymous]
        public async Task<ActionResult> ChangePasswordWithFirebaseUid(string uid, ChangeUserPasswordDto password)
        {
            return Ok(await _teacherService.ChangePasswordWithFirebaseId(uid, password));
        }
    }
}