using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.RegistrationRequestDto;
using griffined_api.Dtos.ScheduleDtos;

namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/v1/schedule")]

    [Authorize(Roles = "ea, master")]

    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;

        }
        [HttpPost("group"), Authorize(Roles = "ec, master")]
        public async Task<ActionResult> AddGroupSchedule(GroupScheduleRequestDto newRequestedSchedule)
        {
            return Ok(await _scheduleService.AddGroupSchedule(newRequestedSchedule));
        }

        [HttpGet("study-courses"), Authorize(Roles = "ec, oa, master")]
        public async Task<ActionResult> GetAllStudyCourse()
        {
            return Ok(await _scheduleService.GetAllStudyCourse());
        }
    }
}