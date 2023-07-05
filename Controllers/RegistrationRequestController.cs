using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.RegistrationRequestDto;

namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/v1/request/registration")]

    [Authorize(Roles = "ea, master")]

    public class RegistrationRequestController : ControllerBase
    {
        private readonly IRegistrationRequestService _registrationRequestService;
        public RegistrationRequestController(IRegistrationRequestService registrationRequestService)
        {
            _registrationRequestService = registrationRequestService;

        }
        [HttpPost("new-course"), Authorize(Roles = "ep, master")]
        public async Task<ActionResult> AddNewCoursesRequest(NewCoursesRequestDto newCourses)
        {
            var response = await _registrationRequestService.AddNewRequestedCourses(newCourses);
            return Ok(response);
        }
        [HttpPost("student-adding"), Authorize(Roles = "ep, master")]
        public async Task<ActionResult> AddStudentAddingRequest(StudyAddingRequestDto newStudentAdding)
        {
            var response = await _registrationRequestService.AddStudentAddingRequest(newStudentAdding);
            return Ok(response);
        }
    }
}