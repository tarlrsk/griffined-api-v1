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
        // [HttpGet, Authorize(Roles = "oa, master, allstaff")]
        [HttpGet, AllowAnonymous]
        public async Task<ActionResult> Get()
        {
            return Ok(await _staffService.GetStaff());
        }

        // [HttpGet("{id}"), Authorize(Roles = "ec, ea, oa, master, allstaff")]
        [HttpGet("{id}"), AllowAnonymous]
        public async Task<ActionResult> GetStaffById(int id)
        {
            var response = await _staffService.GetStaffById(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        // [HttpPost, Authorize(Roles = "oa, master, allstaff")]
        [HttpPost, AllowAnonymous]
        public async Task<ActionResult> AddStaff(AddStaffRequestDto newStaff)
        {
            var response = await _staffService.AddStaff(newStaff);
            if (response.Data is null)
                return BadRequest(response);
            return Ok(response);
        }

        // [HttpPut, Authorize(Roles = "oa, master, allstaff")]
        [HttpPut, AllowAnonymous]
        public async Task<ActionResult> UpdateStaff(UpdateStaffRequestDto updatedStaff)
        {
            var response = await _staffService.UpdateStaff(updatedStaff);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        // [HttpDelete("{id}"), Authorize(Roles = "oa, master, allstaff")]
        [HttpDelete("{id}"), AllowAnonymous]
        public async Task<ActionResult> DeleteECById(int id)
        {
            var response = await _staffService.DeleteStaff(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }


        // [HttpPut("activate/{id}"), Authorize(Roles = "oa, master, allstaff")]
        [HttpPut("activate/{id}"), AllowAnonymous]
        public async Task<ActionResult> EnableStudent(int id)
        {
            var response = await _staffService.EnableStaff(id);
            if (response.Success != true)
                return NotFound(response);
            return Ok(response);
        }


        // [HttpPut("deactivate/{id}"), Authorize(Roles = "oa, master, allstaff")]
        [HttpPut("deactivate/{id}"), AllowAnonymous]
        public async Task<ActionResult> DisableStudent(int id)
        {
            var response = await _staffService.DisableStaff(id);
            if (response.Success != true)
                return NotFound(response);
            return Ok(response);
        }

    }
}