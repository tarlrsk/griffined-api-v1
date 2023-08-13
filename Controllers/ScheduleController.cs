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

        [HttpPost("new-study-courses/{requestId}")]
        public async Task<ActionResult> AddNewStudyCourses(List<NewStudyClassScheduleRequestDto> newStudyClasses, int requestId)
        {
            return Ok(await _scheduleService.AddNewStudyClass(newStudyClasses, requestId));
        } 

        [HttpPut("new-study-courses/{requestId}")]
        public async Task<ActionResult> EditNewStudyCourses(EditStudyClassByRegistrationRequestDto request, int requestId)
        {
            return Ok(await _scheduleService.EditStudyClassByRegisRequest(request, requestId));
        }
    }
}