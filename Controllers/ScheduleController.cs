using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/schedule")]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet, Authorize(Roles = "ep, ea, oa")]
        public async Task<ActionResult<ServiceResponse<List<GetScheduleDto>>>> GetSchedule()
        {
            return Ok(await _scheduleService.GetSchedule());
        }

        [HttpGet("{reqId}"), Authorize(Roles = "ea, ep, oa")]
        public async Task<ActionResult<ServiceResponse<GetScheduleDto>>> GetScheduleByRequestId(int reqId)
        {
            var response = await _scheduleService.GetScheduleByRequestId(reqId);
            if (response is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPost, Authorize(Roles = "ea")]
        public async Task<ActionResult<ServiceResponse<GetScheduleDto>>> AddSchedule(AddScheduleDto newSchedule)
        {
            return Ok(await _scheduleService.AddSchedule(newSchedule));
        }

        [HttpPost("class"), Authorize(Roles = "ea")]
        public async Task<ActionResult<ServiceResponse<GetSinglePrivateClassDto>>> AddPrivateClass(AddSinglePrivateClassDto newClass)
        {
            return Ok(await _scheduleService.AddPrivateClass(newClass));
        }

        [HttpPost("list-of-class"), Authorize(Roles = "ea")]
        public async Task<ActionResult<ServiceResponse<List<GetSinglePrivateClassDto>>>> AddListOfPrivateClass(List<AddSinglePrivateClassDto> newClasses)
        {
            return Ok(await _scheduleService.AddListOfPrivateClass(newClasses));
        }

        [HttpPut("class"), Authorize(Roles = "ea")]
        public async Task<ActionResult<ServiceResponse<UpdatePrivateClassDto>>> UpdatePrivateClass(UpdatePrivateClassDto updatedClass)
        {
            var response = await _scheduleService.UpdatePrivateClass(updatedClass);
            if (response is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPut("list-of-class"), Authorize(Roles = "ea")]
        public async Task<ActionResult<ServiceResponse<List<UpdatePrivateClassDto>>>> UpdateListOfPrivateClass(List<UpdatePrivateClassDto> updatedClasses)
        {
            var response = await _scheduleService.UpdateListOfPrivateClass(updatedClasses);
            if (response is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpDelete("{courseId}"), Authorize(Roles = "ea")]
        public async Task<ActionResult<ServiceResponse<List<GetScheduleDto>>>> DeleteSchedule(int courseId)
        {
            var response = await _scheduleService.DeleteSchedule(courseId);
            if (response is null)
                return NotFound(response);
            return response;
        }


        [HttpPut("deactivate/{courseId}"), Authorize(Roles = "ea, ep ,oa")]
        public async Task<ActionResult<ServiceResponse<List<GetScheduleDto>>>> SoftDelete(int courseId)
        {
            var response = await _scheduleService.SoftDeleteSchedule(courseId);
            if (response is null)
                return NotFound(response);
            return response;
        }

        [HttpDelete("class/{classId}"), Authorize(Roles = "ea")]
        public async Task<ActionResult<ServiceResponse<List<GetPrivateClassDto>>>> DeletePrivateClass(int classId)
        {
            var response = await _scheduleService.DeletePrivateClass(classId);
            if (response is null)
                return NotFound(response);
            return response;
        }
        [HttpDelete("list-of-class"), Authorize(Roles = "ea")]
        public async Task<ActionResult<ServiceResponse<List<GetPrivateClassDto>>>> DeleteListOfPrivateClass([FromQuery] int[] listOfClassId)
        {
            var response = await _scheduleService.DeleteListOfPrivateClass(listOfClassId);
            if (response is null)
                return NotFound(response);
            return response;
        }
    }
}