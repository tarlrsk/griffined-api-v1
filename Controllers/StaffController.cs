using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class StaffController : ControllerBase
    {
        public readonly IStaffService _staffService;
        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }
        [HttpGet("Get"), Authorize(Roles = "oa")]
        public async Task<ActionResult<ServiceResponse<List<GetStaffDto>>>> Get()
        {
            return Ok(await _staffService.GetStaff());
        }

        [HttpGet("Get/{id}"), Authorize(Roles = "ep, ea, oa")]
        public async Task<ActionResult<ServiceResponse<List<GetStaffDto>>>> GetStaffById(int id)
        {
            var response = await _staffService.GetStaffById(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPost("Post"), Authorize(Roles = "oa")]
        public async Task<ActionResult<ServiceResponse<List<GetStaffDto>>>> AddStaff(AddStaffDto newStaff)
        {
            var response = await _staffService.AddStaff(newStaff);
            if (response.Data is null)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpPut("Put"), Authorize(Roles = "oa")]
        public async Task<ActionResult<ServiceResponse<List<GetStaffDto>>>> UpdateStaff(UpdateStaffDto updatedStaff)
        {
            var response = await _staffService.UpdateStaff(updatedStaff);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpDelete("Delete/{id}"), Authorize(Roles = "oa")]
        public async Task<ActionResult<ServiceResponse<GetStaffDto>>> DeleteEPById(int id)
        {
            var response = await _staffService.DeleteStaff(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }


        [HttpGet("Enable/{id}"), Authorize(Roles = "oa")]
        public async Task<ActionResult<ServiceResponse<GetStudentDto>>> EnableStudent(int id)
        {
            var response = await _staffService.EnableStaff(id);
            if (response.Success != true)
                return NotFound(response);
            return Ok(response);
        }


        [HttpDelete("Disable/{id}"), Authorize(Roles = "oa")]
        public async Task<ActionResult<ServiceResponse<GetStudentDto>>> DisableStudent(int id)
        {
            var response = await _staffService.DisableStaff(id);
            if (response.Success != true)
                return NotFound(response);
            return Ok(response);
        }

    }
}