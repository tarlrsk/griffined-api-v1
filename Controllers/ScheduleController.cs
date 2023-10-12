using griffined_api.Services.StudentReportService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Rpc;
using griffined_api.Dtos.StudentReportDtos;
using griffined_api.Dtos.ScheduleDtos;

namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/v1/schedule")]

    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet("today/{date}"), Authorize(Roles = "teacher, student")]
        public async Task<ActionResult> GetToday(string date)
        {
            return Ok(await _scheduleService.GetMobileTodayClass(date));
        }
        
        [HttpGet("staff/{date}"), Authorize(Roles = "ec, ea, oa, master")]
        public async Task<ActionResult> GetDailyCalendarForStaff(string date)
        {
            return Ok(await _scheduleService.GetDailyCalendarForStaff(date));
        }

        [HttpPut("studyclass/room"), Authorize(Roles = "ec, ea, oa, master")]
        public async Task<ActionResult> UpdateStudyClassRoomByScheduleIds(List<UpdateRoomRequestDto> requestDto)
        {
            return Ok(await _scheduleService.UpdateStudyClassRoomByScheduleIds(requestDto));
        }
    }
}