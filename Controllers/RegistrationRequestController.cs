using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.RegistrationRequestDto;

namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/v1/registration/request/new-course")]

    [Authorize(Roles = "ea, master")]

    public class RegistrationRequestController : ControllerBase
    {
        private readonly IRegistrationRequestService _registrationRequestService;
        public RegistrationRequestController(IRegistrationRequestService registrationRequestService)
        {
            _registrationRequestService = registrationRequestService;

        }
        [HttpPost, Authorize(Roles = "oc, master")]
        public async Task<ActionResult> AddNewCoursesRequest(NewCoursesRequestDto newCourses)
        {
            var response = await _registrationRequestService.AddNewCourses(newCourses);
            return Ok(newCourses);
        }

    }
}