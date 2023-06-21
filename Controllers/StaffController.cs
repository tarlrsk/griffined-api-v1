using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/v1/staff")]

    public class StaffController : ControllerBase
    {
        public readonly IStaffService _staffService;
        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }
        [HttpGet, Authorize(Roles = "oa, master")]
        public async Task<ActionResult<ServiceResponse<List<GetStaffDto>>>> Get()
        {
            return Ok(await _staffService.GetStaff());
        }

        [HttpGet("{id}"), Authorize(Roles = "ep, ea, oa, master")]
        public async Task<ActionResult<ServiceResponse<List<GetStaffDto>>>> GetStaffById(int id)
        {
            var response = await _staffService.GetStaffById(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetStaffDto>>>> AddStaff(AddStaffDto newStaff)
        {
            var response = await _staffService.AddStaff(newStaff);
            if (response.Data is null)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpPut, Authorize(Roles = "oa, master")]
        public async Task<ActionResult<ServiceResponse<List<GetStaffDto>>>> UpdateStaff(UpdateStaffDto updatedStaff)
        {
            var response = await _staffService.UpdateStaff(updatedStaff);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpDelete("{id}"), Authorize(Roles = "oa, master")]
        public async Task<ActionResult<ServiceResponse<GetStaffDto>>> DeleteEPById(int id)
        {
            var response = await _staffService.DeleteStaff(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }


        [HttpPut("activate/{id}"), Authorize(Roles = "oa, master")]
        public async Task<ActionResult<ServiceResponse<GetStudentDto>>> EnableStudent(int id)
        {
            var response = await _staffService.EnableStaff(id);
            if (response.Success != true)
                return NotFound(response);
            return Ok(response);
        }


        [HttpPut("deactivate/{id}"), Authorize(Roles = "oa, master")]
        public async Task<ActionResult<ServiceResponse<GetStudentDto>>> DisableStudent(int id)
        {
            var response = await _staffService.DisableStaff(id);
            if (response.Success != true)
                return NotFound(response);
            return Ok(response);
        }

    }
}