using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.RegistrationRequestDto;
using Newtonsoft.Json;

namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/v1/request/registration")]

    public class RegistrationRequestController : ControllerBase
    {
        private readonly IRegistrationRequestService _registrationRequestService;
        public RegistrationRequestController(IRegistrationRequestService registrationRequestService)
        {
            _registrationRequestService = registrationRequestService;

        }
        [HttpPost("new-course"), Authorize(Roles = "ec, master")]
        public async Task<ActionResult> AddNewCoursesRequest(NewCoursesRequestDto newCourses)
        {
            var response = await _registrationRequestService.AddNewRequestedCourses(newCourses);
            return Ok(response);
        }
        [HttpPost("student-adding"), Authorize(Roles = "ec, master")]
        public async Task<ActionResult> AddStudentAddingRequest([FromForm]StudentAddingRequestDto newStudentAdding, List<IFormFile> filesToUpload)
        {
            var response = await _registrationRequestService.AddStudentAddingRequest(newStudentAdding, filesToUpload);
            return Ok(response);
        }

        [HttpPost("student-adding-backup"), Authorize(Roles = "ec, master")]
        public async Task<ActionResult> AddStudentAddingRequest2([FromForm] string newStudentAdding, List<IFormFile> filesToUpload)
        {
            var newStudentAddingDto = JsonConvert.DeserializeObject<StudentAddingRequestDto>(newStudentAdding);
            if (newStudentAddingDto == null)
                throw new InternalServerException("Cannot Map Object");
            var response = await _registrationRequestService.AddStudentAddingRequest(newStudentAddingDto, filesToUpload);
            return Ok(response);
        }
        

        [HttpGet, Authorize(Roles = "ec, ea, oa, master")]
        public async Task<ActionResult> GetAllRegistrationRequest()
        {
            var response = await _registrationRequestService.ListRegistrationRequests();
            return Ok(response);
        }

        [HttpGet("pending-ea/{requestId}"), Authorize(Roles = "ea, ec, master")]
        public async Task<ActionResult> GetPendingEADetail(int requestId)
        {
            var response = await _registrationRequestService.GetPendingEADetail(requestId);
            return Ok(response);
        }

        [HttpGet("pending-ec/{requestId}"), Authorize(Roles = "ea, ec, master")]
        public async Task<ActionResult> GetPendingECDetail(int requestId)
        {
            var response = await _registrationRequestService.GetPendingECDetail(requestId);
            return Ok(response);
        }

        [HttpPut("submit-payment/{requestId}"), Authorize(Roles = "ec, master")]
        public async Task<ActionResult> SubmitPayment(int requestId, [FromForm] SubmitPaymentRequestDto request, List<IFormFile> filesToUpload)
        {
            var response = await _registrationRequestService.SubmitPayment(requestId, request, filesToUpload);
            return Ok(response);
        }

        [HttpGet("pending-oa/{requestId}"), Authorize(Roles = "ec, oa, master")]
        public async Task<ActionResult> GetPendingOADetail(int requestId)
        {
            var response = await _registrationRequestService.GetPendingOADetail(requestId);
            return Ok(response);
        }

        [HttpPut("approve-payment/{requestId}/{paymentStatus}"), Authorize(Roles = "oa, master")]
        public async Task<ActionResult> ApprovePayment(int requestId, PaymentStatus paymentStatus)
        {
            var response = await _registrationRequestService.ApprovePayment(requestId, paymentStatus);
            return Ok(response);
        }

        [HttpPut("decline-payment/{requestId}"), Authorize(Roles = "oa, master")]
        public async Task<ActionResult> DeclinePayment(int requestId)
        {
            var response = await _registrationRequestService.DeclinePayment(requestId);
            return Ok(response);
        }
    }
}