using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "ep, ea, oa")]
    public class PrivateRegistrationRequestController : ControllerBase
    {
        private readonly IPrivateRegistrationRequestService _privateRegistrationRequestService;
        public PrivateRegistrationRequestController(IPrivateRegistrationRequestService privateRegistrationRequestService)
        {
            _privateRegistrationRequestService = privateRegistrationRequestService;
        }

        [HttpGet("Get")]
        public async Task<ActionResult<ServiceResponse<List<GetPrivateRegReqWithInfoDto>>>> GetPrivateRegistrationRequest()
        {
            var response = await _privateRegistrationRequestService.GetPrivateRegistrationRequest();
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("Get/{reqId}")]
        public async Task<ActionResult<ServiceResponse<GetPrivateRegReqWithInfoDto>>> GetPrivateRegistrationRequestById(int reqId)
        {
            var response = await _privateRegistrationRequestService.GetPrivateRegistrationRequestById(reqId);
            if (response.Data is null)
                return NotFound(response);
            return response;
        }

        [HttpPost("Post"), Authorize(Roles = "ep")]
        public async Task<ActionResult<ServiceResponse<List<GetPrivateRegReqWithInfoDto>>>> AddPrivateRegistrationRequest(AddPrivateRegReqWithInfoDto newRequest)
        {
            var response = await _privateRegistrationRequestService.AddPrivateRegistrationRequest(newRequest);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpPut("Put")]
        public async Task<ActionResult<ServiceResponse<GetPrivateRegReqWithInfoDto>>> UpdatePrivateRegistrationRequest(UpdatePrivateRegReqWithInfoDto updatedRequest)
        {
            var response = await _privateRegistrationRequestService.UpdatePrivateRegistrationRequest(updatedRequest);
            if (updatedRequest is null)
                return NotFound(response);
            return response;
        }

        [HttpDelete("Delete/{reqId}")]
        public async Task<ActionResult<ServiceResponse<List<GetPrivateRegReqWithInfoDto>>>> DeletePrivateRegistrationRequest(int reqId)
        {
            var response = await _privateRegistrationRequestService.DeletePrivateRegistrationRequest(reqId);
            if (response.Data is null)
                return NotFound(response);
            return response;
        }
    }
}