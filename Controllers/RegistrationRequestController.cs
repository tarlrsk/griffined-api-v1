using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.CommentDtos;
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
        public async Task<ActionResult> AddStudentAddingRequest([FromForm] StudentAddingRequestDto newStudentAdding, [Required] List<IFormFile> filesToUpload)
        {
            var response = await _registrationRequestService.AddStudentAddingRequest(newStudentAdding, filesToUpload);
            return Ok(response);
        }

        [HttpPost("student-adding-backup"), Authorize(Roles = "ec, master")]
        public async Task<ActionResult> AddStudentAddingRequest2([FromForm] string newStudentAdding, [Required] List<IFormFile> filesToUpload)
        {
            var newStudentAddingDto = JsonConvert.DeserializeObject<StudentAddingRequestDto>(newStudentAdding) ?? throw new InternalServerException("Cannot Map Object");
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

        [HttpPut("decline-schedule/{requestId}"), Authorize(Roles = "ec, master")]
        public async Task<ActionResult> DeclineSchedule(int requestId)
        {
            var response = await _registrationRequestService.DeclineSchedule(requestId);
            return Ok(response);
        }

        [HttpGet("pending-ea2/{requestId}"), Authorize(Roles = "ea, ec, master")]
        public async Task<ActionResult> GetPendingEADetail2(int requestId)
        {
            var response = await _registrationRequestService.GetPendingEADetail2(requestId);
            return Ok(response);
        }

        [HttpGet("pending-ec/{requestId}"), Authorize(Roles = "ea, ec, oa, master")]
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

        [HttpPut("cancel/{requestId}"), Authorize(Roles = "ec,ea, master")]
        public async Task<ActionResult> CancelRequest(int requestId)
        {
            var response = await _registrationRequestService.CancelRequest(requestId);
            return Ok(response);
        }

        [HttpPut("payment/{requestId}"), Authorize(Roles = "ec, oa, master")]
        public async Task<ActionResult> UpdatePayment(int requestId, [FromForm] UpdatePaymentRequestDto updatePaymentRequest)
        {
            return Ok(await _registrationRequestService.UpdatePayment(requestId, updatePaymentRequest));
        }

        [HttpGet("completed/{requestId}"), Authorize(Roles = "ec, ea, oa, master")]
        public async Task<ActionResult> GetCompletedRequest(int requestId)
        {
            return Ok(await _registrationRequestService.GetCompletedRequest(requestId));
        }

        [HttpGet("cancelled/{requestId}"), Authorize(Roles = "ec, ea, oa, master")]
        public async Task<ActionResult> GetCancellationRequest(int requestId)
        {
            return Ok(await _registrationRequestService.GetCancellationRequest(requestId));
        }

        [HttpPut("take/{requestId}"), Authorize(Roles = "ea, master")]
        public async Task<ActionResult> EaTakeRequest(int requestId)
        {
            return Ok(await _registrationRequestService.EaTakeRequest(requestId));
        }

        [HttpPut("release/{requestId}"), Authorize(Roles = "ea, master")]
        public async Task<ActionResult> EaReleaseRequest(int requestId)
        {
            return Ok(await _registrationRequestService.EaReleaseRequest(requestId));
        }

        [HttpPost("comment/{requestId}"), Authorize(Roles = "ec, ea, oa, master")]
        public async Task<ActionResult> AddComment(int requestId, CommentRequestDto comment)
        {
            return Ok(await _registrationRequestService.AddComment(requestId, comment));
        }

        [HttpGet("comment/{requestId}"), Authorize(Roles = "ec, ea, oa, master")]
        public async Task<ActionResult> GetCommentsByRequestId(int requestId)
        {
            return Ok(await _registrationRequestService.GetCommentsByRequestId(requestId));
        }
    }
}